export interface TreeVisitor<T> {
  visit(node: TreeNode<T>, depth: number): void;
}

export class TreeNode<T> {
  private readonly _children: TreeNode<T>[] = [];
  constructor(public value: T) {}

  public get children(): readonly TreeNode<T>[] {
    return this._children;
  }

  public addChild(child: TreeNode<T>): TreeNode<T> {
    this._children.push(child);
    return child;
  }

  public removeChild(child: TreeNode<T>): boolean {
    const index = this._children.indexOf(child);
    if (index === -1) return false;
    this._children.splice(index, 1);
    return true;
  }
}

export class Tree<T> {
  constructor(public readonly root: TreeNode<T>) {}

  public traverseDFS(action: ((node: TreeNode<T>, depth: number) => void) | TreeVisitor<T>): void {
    const visitFn = typeof action === 'function' ? action : action.visit.bind(action);

    const walk = (node: TreeNode<T>, depth: number): void => {
      visitFn(node, depth);
      for (const child of node.children) {
        walk(child, depth + 1);
      }
    };

    walk(this.root, 0);
  }

  public traverseBFS(action: ((node: TreeNode<T>, depth: number) => void) | TreeVisitor<T>): void {
    const visitFn = typeof action === 'function' ? action : action.visit.bind(action);
    const queue: Array<{ node: TreeNode<T>; depth: number }> = [{ node: this.root, depth: 0 }];

    while (queue.length > 0) {
      const current = queue.shift()!;
      visitFn(current.node, current.depth);
      for (const child of current.node.children) {
        queue.push({ node: child, depth: current.depth + 1 });
      }
    }
  }
}