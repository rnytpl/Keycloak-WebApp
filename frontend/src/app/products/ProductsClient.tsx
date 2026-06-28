"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  isListed: boolean;
};

interface ProductsClientProps {
  initialProducts: Product[];
  roles: string[];
  token: string;
}

export const ProductsClient = ({ initialProducts, roles, token }: ProductsClientProps) => {
  const [products, setProducts] = useState<Product[]>(initialProducts);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [newProduct, setNewProduct] = useState({ name: "", description: "", price: 0 });

  const isAdmin = roles.includes("admin");
  const isModerator = roles.includes("moderator");
  const canCreate = isAdmin || isModerator;

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    const res = await fetch("http://localhost:5212/api/products", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(newProduct),
    });

    if (res.ok) {
      const id = await res.json();
      setProducts([...products, { ...newProduct, id, isListed: true }]);
      setIsCreateOpen(false);
      setNewProduct({ name: "", description: "", price: 0 });
    } else {
      alert("Failed to create product");
    }
  };

  const handleDelete = async (id: number) => {
    if (!isAdmin) return;
    const res = await fetch(`http://localhost:5212/api/products/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    if (res.ok) {
      setProducts(products.filter(p => p.id !== id));
    } else {
      alert("Failed to delete");
    }
  };

  const handleToggleStatus = async (product: Product) => {
    if (!isAdmin) return;
    const res = await fetch(`http://localhost:5212/api/products/${product.id}/status`, {
      method: "PATCH",
      headers: { 
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}` 
      },
      body: JSON.stringify({ id: product.id, isListed: !product.isListed })
    });
    
    if (res.ok) {
      setProducts(products.map(p => p.id === product.id ? { ...p, isListed: !p.isListed } : p));
    } else {
      alert("Failed to update status");
    }
  };

  return (
    <div className="space-y-4">
      {canCreate && (
        <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
          <DialogTrigger render={<Button />}>
            Create Product
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create New Product</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleCreate} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">Name</Label>
                <Input id="name" value={newProduct.name} onChange={e => setNewProduct({...newProduct, name: e.target.value})} required />
              </div>
              <div className="space-y-2">
                <Label htmlFor="desc">Description</Label>
                <Input id="desc" value={newProduct.description} onChange={e => setNewProduct({...newProduct, description: e.target.value})} required />
              </div>
              <div className="space-y-2">
                <Label htmlFor="price">Price</Label>
                <Input id="price" type="number" step="0.01" value={newProduct.price} onChange={e => setNewProduct({...newProduct, price: parseFloat(e.target.value)})} required />
              </div>
              <Button type="submit" className="w-full">Save Product</Button>
            </form>
          </DialogContent>
        </Dialog>
      )}

      <div className="border rounded-md">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Description</TableHead>
              <TableHead>Price</TableHead>
              <TableHead>Status</TableHead>
              {(isAdmin) && <TableHead className="text-right">Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {products.length === 0 ? (
              <TableRow>
                <TableCell colSpan={isAdmin ? 5 : 4} className="text-center py-8 text-muted-foreground">
                  No products available.
                </TableCell>
              </TableRow>
            ) : products.map(product => (
              <TableRow key={product.id}>
                <TableCell className="font-medium">{product.name}</TableCell>
                <TableCell>{product.description}</TableCell>
                <TableCell>\${product.price.toFixed(2)}</TableCell>
                <TableCell>
                  <Badge variant={product.isListed ? "default" : "destructive"}>
                    {product.isListed ? "Listed" : "Delisted"}
                  </Badge>
                </TableCell>
                {isAdmin && (
                  <TableCell className="text-right space-x-2">
                    <Button variant="outline" size="sm" onClick={() => handleToggleStatus(product)}>
                      Toggle Status
                    </Button>
                    <Button variant="destructive" size="sm" onClick={() => handleDelete(product.id)}>
                      Delete
                    </Button>
                  </TableCell>
                )}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
};
