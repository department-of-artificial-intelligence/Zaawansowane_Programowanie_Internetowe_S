import { TreeNode } from "./TreeNode";

export interface TreeVisitor<T> {
    visit(node: TreeNode<T>): void;
}
