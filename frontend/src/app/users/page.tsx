import { getServerSession } from "next-auth";
import { authOptions } from "../api/auth/[...nextauth]/route";
import { redirect } from "next/navigation";
import { UsersClient } from "./UsersClient";

export const dynamic = "force-dynamic";

export default async function UsersPage() {
  const session = await getServerSession(authOptions);
  
  if (!session) {
    redirect("/api/auth/signin");
  }

  // @ts-ignore
  const roles = session.roles || [];
  if (!roles.includes("admin") && !roles.includes("moderator")) {
    return <div className="p-8 text-center text-red-500">You do not have permission to view users.</div>;
  }

  // @ts-ignore
  const token = session.accessToken;
  
  const res = await fetch("http://localhost:5212/api/users", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!res.ok) {
    return <div className="p-8 text-center text-red-500">Failed to load users.</div>;
  }

  const users = await res.json();

  return (
    <main className="p-4 sm:p-8 md:p-12 min-h-screen bg-background text-foreground">
      <div className="max-w-6xl mx-auto space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Registered Users</h1>
          <p className="text-muted-foreground mt-2">A list of all users currently registered in Keycloak.</p>
        </div>

        <UsersClient initialUsers={users} roles={roles} token={token} />
      </div>
    </main>
  );
}
