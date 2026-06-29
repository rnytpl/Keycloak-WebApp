import { getServerSession } from "next-auth";
import { authOptions } from "../api/auth/[...nextauth]/route";
import { redirect } from "next/navigation";
import { ProductsClient } from "./ProductsClient";

export const dynamic = "force-dynamic";

const ProductsPage = async () => {
  const session = await getServerSession(authOptions);
  
  if (!session) {
    redirect("/api/auth/signin");
  }

  // @ts-expect-error custom session type
  const token = session.accessToken;
  
  const res = await fetch("http://localhost:5212/api/products", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!res.ok) {
    return <div className="p-8 text-center text-red-500">Failed to load products. Backend might be down or you lack access.</div>;
  }

  const products = await res.json();
  // @ts-expect-error custom session type
  const roles = session.roles || [];

  return (
    <main className="p-4 sm:p-8 md:p-12 min-h-screen bg-background text-foreground">
      <div className="max-w-6xl mx-auto space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Products</h1>
          <p className="text-muted-foreground mt-2">Manage your product catalog based on your role.</p>
        </div>
        <ProductsClient initialProducts={products} roles={roles} token={token} />
      </div>
    </main>
  );
};

export default ProductsPage;
