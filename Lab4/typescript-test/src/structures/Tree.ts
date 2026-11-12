export class TreeNode<T> {
    public value: T;
    public children: TreeNode<T>[] = [];

    constructor(value: T) {
        this.value = value;
    }

    public addChild(child: TreeNode<T> | T): TreeNode<T> {
        const node = child instanceof TreeNode ? child : new TreeNode(child);
        this.children.push(node);
        return node;
    }
}

export class Tree<T> {
    public root: TreeNode<T> | null = null;

    constructor(rootValue?: T) {
        if (rootValue !== undefined) {
            this.root = new TreeNode(rootValue);
        }
    }

    public traverseDFS(callback: (val: T, node: TreeNode<T>) => void): void {
        if (!this.root) return;

        const walk = (node: TreeNode<T>) => {
            callback(node.value, node);
            node.children.forEach(walk);
        };

        walk(this.root);
    }

    public traverseBFS(callback: (val: T, node: TreeNode<T>) => void): void {
        if (!this.root) return;

        const queue: TreeNode<T>[] = [this.root];

        while (queue.length > 0) {
            const current = queue.shift()!;
            callback(current.value, current);
            queue.push(...current.children);
        }
    }

    public accept(visitor: (node: TreeNode<T>) => void): void {
        this.traverseDFS((_, node) => visitor(node));
    }
}