"use client";

import Link from "next/link";
import { useSession, signOut } from "next-auth/react";
import { Button } from "./ui/button";

export const NavBar = () => {
  const { data: session, status } = useSession();

  const handleSignOut = async () => {
    // @ts-ignore
    const idToken = session?.idToken;
    
    await signOut({ redirect: false });
    
    let logoutUrl = `http://localhost:8080/realms/webapp-realm/protocol/openid-connect/logout`;
    const postLogoutUrl = encodeURIComponent(window.location.origin);
    
    if (idToken) {
      logoutUrl += `?id_token_hint=${idToken}&post_logout_redirect_uri=${postLogoutUrl}`;
    } else {
      logoutUrl += `?client_id=nextjs-client&post_logout_redirect_uri=${postLogoutUrl}`;
    }
    
    window.location.href = logoutUrl;
  };

  return (
    <nav className="border-b bg-background sticky top-0 z-50">
      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex items-center space-x-8">
            <Link href="/" className="text-xl font-bold tracking-tight text-foreground">
              KeycloakApp
            </Link>
            {status === "authenticated" && (
              <div className="hidden md:flex space-x-4 items-center">
                <Link href="/products" className="text-sm font-medium text-muted-foreground hover:text-foreground">
                  Products
                </Link>
                {/* @ts-ignore */}
                {(session?.roles?.includes("admin") || session?.roles?.includes("moderator")) && (
                  <Link href="/users" className="text-sm font-medium text-muted-foreground hover:text-foreground">
                    Users
                  </Link>
                )}
              </div>
            )}
          </div>
          
          <div className="flex items-center space-x-4">
            {status === "authenticated" ? (
              <>
                <span className="text-sm text-muted-foreground hidden sm:inline-block font-medium">
                  {session?.user?.name || session?.user?.email}
                </span>
                <Button variant="outline" size="sm" onClick={handleSignOut}>
                  Log out
                </Button>
              </>
            ) : status === "unauthenticated" ? (
              <Button asChild size="sm">
                <Link href="/api/auth/signin">Log in</Link>
              </Button>
            ) : null}
          </div>
        </div>
      </div>
    </nav>
  );
}
