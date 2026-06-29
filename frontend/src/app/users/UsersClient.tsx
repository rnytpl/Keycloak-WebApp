"use client";

import { useState } from "react";
import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import { MoreHorizontal } from "lucide-react";

type User = {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  enabled: boolean;
  roles: string[];
};

interface UsersClientProps {
  initialUsers: User[];
  roles: string[];
  token: string;
}

export const UsersClient = ({ initialUsers, roles, token }: UsersClientProps) => {
  const [users, setUsers] = useState<User[]>(initialUsers);
  const isAdmin = roles.includes("admin");

  const handleAssignRole = async (userId: string, roleName: string) => {
    const res = await fetch(`http://localhost:5212/api/users/${userId}/roles`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(roleName)
    });
    
    if (res.ok) {
      setUsers(users.map(u => u.id === userId ? { ...u, roles: [...(u.roles || []), roleName] } : u));
    } else {
      alert("Failed to assign role");
    }
  };

  const handleRemoveRole = async (userId: string, roleName: string) => {
    const res = await fetch(`http://localhost:5212/api/users/${userId}/roles/${roleName}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    
    if (res.ok) {
      setUsers(users.map(u => u.id === userId ? { ...u, roles: (u.roles || []).filter(r => r !== roleName) } : u));
    } else {
      alert("Failed to remove role");
    }
  };

  return (
    <div className="border rounded-md">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Username</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>First Name</TableHead>
            <TableHead>Last Name</TableHead>
            <TableHead>Roles</TableHead>
            <TableHead>Status</TableHead>
            {isAdmin && <TableHead className="text-right">Actions</TableHead>}
          </TableRow>
        </TableHeader>
        <TableBody>
          {users.map((user) => (
            <TableRow key={user.id}>
              <TableCell className="font-medium">
                <Link href={`/users/${user.id}`} className="hover:underline text-primary">
                  {user.username}
                </Link>
              </TableCell>
              <TableCell>{user.email}</TableCell>
              <TableCell>{user.firstName}</TableCell>
              <TableCell>{user.lastName}</TableCell>
              <TableCell className="space-x-1">
                {user.roles && user.roles.filter(r => r !== "uma_authorization" && r !== "offline_access").map(role => (
                  <Badge key={role} variant={role === "admin" ? "destructive" : role === "moderator" ? "default" : "secondary"}>
                    {role}
                  </Badge>
                ))}
              </TableCell>
              <TableCell>
                <Badge variant={user.enabled ? "default" : "secondary"}>
                  {user.enabled ? "Active" : "Disabled"}
                </Badge>
              </TableCell>
              {isAdmin && (
                <TableCell className="text-right">
                  <DropdownMenu>
                    <DropdownMenuTrigger render={<Button variant="ghost" className="h-8 w-8 p-0" />}>
                      <span className="sr-only">Open menu</span>
                      <MoreHorizontal className="h-4 w-4" />
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      {!user.roles?.includes("admin") && (
                        <DropdownMenuItem onClick={() => handleAssignRole(user.id, "admin")}>
                          Make Admin
                        </DropdownMenuItem>
                      )}
                      {!user.roles?.includes("moderator") && (
                        <DropdownMenuItem onClick={() => handleAssignRole(user.id, "moderator")}>
                          Make Moderator
                        </DropdownMenuItem>
                      )}
                      {user.roles?.includes("admin") && (
                        <DropdownMenuItem onClick={() => handleRemoveRole(user.id, "admin")} className="text-red-500">
                          Remove Admin
                        </DropdownMenuItem>
                      )}
                      {user.roles?.includes("moderator") && (
                        <DropdownMenuItem onClick={() => handleRemoveRole(user.id, "moderator")} className="text-red-500">
                          Remove Moderator
                        </DropdownMenuItem>
                      )}
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              )}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};
