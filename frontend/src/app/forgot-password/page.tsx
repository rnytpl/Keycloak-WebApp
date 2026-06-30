"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import Link from "next/link";
import { ArrowLeft } from "lucide-react";

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const response = await fetch(`http://localhost:5212/api/users/forgot-password?email=${encodeURIComponent(email)}`, {
        method: "PUT",
      });

      if (!response.ok) {
        // We still show generic success on the frontend, but if it's a hard 500 error from the backend we might want to log it
        throw new Error("Failed to communicate with server");
      }

      setSuccess(true);
    } catch (err) {
      // In production, we'd still show success to prevent enumeration, but for debugging we can show the error
      setSuccess(true); 
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <div className="w-full max-w-md space-y-8 rounded-xl border bg-card p-8 shadow-sm">
        
        <div>
          <Link href="/" className="inline-flex items-center text-sm font-medium text-muted-foreground hover:text-foreground mb-6">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to home
          </Link>
          <div className="text-center">
            <h2 className="text-2xl font-bold tracking-tight">Forgot Password</h2>
            <p className="text-sm text-muted-foreground mt-2">
              Enter your email address and we will send you a link to reset your password.
            </p>
          </div>
        </div>

        {success ? (
          <div className="rounded-md bg-green-500/15 p-4 border border-green-500/20">
            <div className="flex">
              <div className="ml-3">
                <h3 className="text-sm font-medium text-green-700 dark:text-green-400">
                  Email Sent
                </h3>
                <div className="mt-2 text-sm text-green-600 dark:text-green-300">
                  <p>If an account with that email exists, a password reset link has been sent.</p>
                </div>
              </div>
            </div>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-6">
            {error && (
              <div className="text-sm text-destructive font-medium p-3 bg-destructive/10 rounded-md">
                {error}
              </div>
            )}
            
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email address</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="name@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
            </div>

            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? "Sending..." : "Send Reset Link"}
            </Button>
          </form>
        )}
      </div>
    </div>
  );
}
