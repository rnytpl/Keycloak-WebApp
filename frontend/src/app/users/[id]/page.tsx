import { getServerSession } from "next-auth";
import { authOptions } from "../../api/auth/[...nextauth]/route";
import { redirect } from "next/navigation";
import { Badge } from "@/components/ui/badge";
import Link from "next/link";
import { Button } from "@/components/ui/button";

type User = {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  enabled: boolean;
  roles: string[];
};

export default async function UserProfilePage({ params }: { params: Promise<{ id: string }> }) {
  const session = await getServerSession(authOptions);

  if (!session) {
    redirect("/api/auth/signin/keycloak");
  }

  // @ts-expect-error custom session augmentation
  const token = session.accessToken;

  const { id } = await params;

  const res = await fetch(`http://localhost:5212/api/users/${id}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!res.ok) {
    if (res.status === 401) {
      redirect("/api/auth/signin/keycloak");
    }
    if (res.status === 404) {
        return <div className="p-8 text-center text-red-500">User not found.</div>;
    }
    return <div className="p-8 text-center text-red-500">Failed to load user details.</div>;
  }

  const user: User = await res.json();

  return (
    <main className="p-4 sm:p-8 md:p-12 min-h-screen bg-background text-foreground">
      <div className="max-w-3xl mx-auto space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">User Profile</h1>
            <p className="text-muted-foreground mt-2">Detailed information for {user.username}.</p>
          </div>
          <Link href="/users">
            <Button variant="outline">Back to Users</Button>
          </Link>
        </div>

        {/* Card */}
        <div className="rounded-xl border bg-card text-card-foreground shadow">
          <div className="flex flex-col space-y-1.5 p-6 border-b">
            <h3 className="font-semibold leading-none tracking-tight text-xl">Account Details</h3>
          </div>
          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-y-6 gap-x-4">
              <div>
                <p className="text-sm font-medium text-muted-foreground">ID</p>
                <p className="mt-1 font-mono text-sm">{user.id}</p>
              </div>
              
              <div>
                <p className="text-sm font-medium text-muted-foreground">Status</p>
                <div className="mt-1">
                  <Badge variant={user.enabled ? "default" : "secondary"}>
                    {user.enabled ? "Active" : "Disabled"}
                  </Badge>
                </div>
              </div>

              <div>
                <p className="text-sm font-medium text-muted-foreground">Username</p>
                <p className="mt-1">{user.username}</p>
              </div>

              <div>
                <p className="text-sm font-medium text-muted-foreground">Email</p>
                <p className="mt-1">{user.email || "N/A"}</p>
              </div>

              <div>
                <p className="text-sm font-medium text-muted-foreground">First Name</p>
                <p className="mt-1">{user.firstName || "N/A"}</p>
              </div>

              <div>
                <p className="text-sm font-medium text-muted-foreground">Last Name</p>
                <p className="mt-1">{user.lastName || "N/A"}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Roles Card */}
        <div className="rounded-xl border bg-card text-card-foreground shadow">
          <div className="flex flex-col space-y-1.5 p-6 border-b">
            <h3 className="font-semibold leading-none tracking-tight text-xl">Assigned Roles</h3>
          </div>
          <div className="p-6">
            {user.roles && user.roles.length > 0 ? (
              <div className="flex flex-wrap gap-2">
                {user.roles.filter(r => r !== "uma_authorization" && r !== "offline_access").map(role => (
                  <Badge key={role} variant={role === "admin" ? "destructive" : role === "moderator" ? "default" : "secondary"}>
                    {role}
                  </Badge>
                ))}
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">No roles assigned.</p>
            )}
          </div>
        </div>
      </div>
    </main>
  );
}
