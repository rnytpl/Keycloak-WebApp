"use client";

import { signIn, useSession } from "next-auth/react";
import { Button } from "@/components/ui/button";

const Home = () => {
  const { data: session, status } = useSession();

  return (
    <main className="flex min-h-screen flex-col items-center justify-center bg-background p-4 sm:p-8 md:p-24">
      <div className="w-full max-w-3xl space-y-6 text-center px-4">
        <h1 className="text-4xl font-extrabold tracking-tight lg:text-5xl">
          Keycloak Web App
        </h1>

        {status === "loading" && (
          <p className="text-lg text-muted-foreground">Loading session...</p>
        )}

        {status === "unauthenticated" && (
          <div className="space-y-4">
            <p className="text-lg text-muted-foreground">
              You are currently not logged in.
            </p>
            <div className="flex flex-col sm:flex-row justify-center gap-4">
              <Button size="lg" className="w-full sm:w-auto" onClick={() => signIn("keycloak")}>
                Log in with Keycloak
              </Button>
              <Button size="lg" variant="outline" className="w-full sm:w-auto" onClick={() => window.location.href = "/register"}>
                Register Account
              </Button>
            </div>
          </div>
        )}

        {status === "authenticated" && (
          <div className="space-y-4">
            <p className="text-lg font-medium text-primary">
              Welcome, {session.user?.name}!
            </p>
            <p className="text-sm text-muted-foreground break-all max-w-xl mx-auto">
              Your Access Token: <br />
              <span className="font-mono bg-muted p-2 rounded block mt-2 max-h-32 overflow-y-auto">
                {/* @ts-expect-error custom session type */}
                {session.accessToken}
              </span>
            </p>
          </div>
        )}
      </div>
    </main>
  );
};

export default Home;
