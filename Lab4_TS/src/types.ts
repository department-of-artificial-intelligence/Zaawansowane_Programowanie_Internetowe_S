import type { Artykul } from "./classes/artykul";
import type { Autor } from "./classes/autor";
import type { Blog } from "./classes/blog";
import type { TreeNode } from "./classes/tree";

export type AutorViewMode = "bullet" | "numbered" | "table";

export type TreeTraverseAction<T> = (node: TreeNode<T>, depth: number) => void;

export type AutorDTO = Omit<Autor, "id">;
export type ArtykulDTO = Omit<Artykul, "id">;
export type BlogDTO = Omit<Blog, "id">;

export type AutorListItem = Pick<Autor, "imie" | "nazwisko">;