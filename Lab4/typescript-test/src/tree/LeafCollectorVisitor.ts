import { TreeNode } from "./TreeNode";
import type { TreeVisitor } from "./TreeVisitor";

// Collects the values of all leaves (nodes without children)
export class LeafCollectorVisitor<T> implements TreeVisitor<T> {
    private _leaves: T[] = [];

    visit(node: TreeNode<T>): void {
        if (node.isLeaf) this._leaves.push(node.value);
    }

    get leaves(): T[] { return [...this._leaves]; }
}
