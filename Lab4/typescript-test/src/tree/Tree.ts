import { TreeNode } from "./TreeNode";
import type { TreeVisitor } from "./TreeVisitor";

export class Tree<T> implements Iterable<T> {
    private _root: TreeNode<T>;

    constructor(rootValue: T) {
        this._root = new TreeNode(rootValue);
    }

    get root(): TreeNode<T> { return this._root; }

    // Visitor - any operation on all nodes without changing the Tree classes
    accept(visitor: TreeVisitor<T>): void {
        this._root.accept(visitor);
    }

    // Depth-first (pre-order) traversal
    traverseDepthFirst(action: (node: TreeNode<T>) => void): void {
        const stack: TreeNode<T>[] = [this._root];
        while (stack.length > 0) {
            const node = stack.pop()!;
            action(node);
            stack.push(...node.children.reverse());
        }
    }

    // Breadth-first (level by level) traversal
    traverseBreadthFirst(action: (node: TreeNode<T>) => void): void {
        const queue: TreeNode<T>[] = [this._root];
        while (queue.length > 0) {
            const node = queue.shift()!;
            action(node);
            queue.push(...node.children);
        }
    }

    find(predicate: (value: T) => boolean): TreeNode<T> | undefined {
        let result: TreeNode<T> | undefined;
        this.traverseDepthFirst(node => {
            if (result === undefined && predicate(node.value)) result = node;
        });
        return result;
    }

    get size(): number {
        let count = 0;
        this.traverseDepthFirst(() => count++);
        return count;
    }

    // Iterator pattern - allows: for (const value of tree) { ... }
    *[Symbol.iterator](): Iterator<T> {
        const stack: TreeNode<T>[] = [this._root];
        while (stack.length > 0) {
            const node = stack.pop()!;
            yield node.value;
            stack.push(...node.children.reverse());
        }
    }
}
