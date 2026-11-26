import type { TreeNode } from "../structures/Tree";

export interface ITreeVisitor<T> {
  visit(node: TreeNode<T>): void;
}
