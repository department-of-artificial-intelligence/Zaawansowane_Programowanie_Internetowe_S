// Interfejs wzorca Wizytator (Visitor Pattern)
export interface ITreeVisitor<T> {
    visit(node: TreeNode<T>, depth: number): void;
}

// 1. Klasa pojedynczego węzła drzewa o typie generycznym T
export class TreeNode<T> {
    public value: T;
    public children: TreeNode<T>[] = [];
    public parent: TreeNode<T> | null = null;

    constructor(value: T) {
        this.value = value;
    }

    // Sprawdza, czy dany węzeł jest liściem (nie ma dzieci)
    public isLeaf(): boolean {
        return this.children.length === 0;
    }

    // Dodaje węzeł potomny
    public addChild(child: TreeNode<T> | T): TreeNode<T> {
        const childNode = child instanceof TreeNode ? child : new TreeNode<T>(child);
        childNode.parent = this;
        this.children.push(childNode);
        return childNode;
    }

    // Usuwa wskazany węzeł potomny
    public removeChild(child: TreeNode<T>): boolean {
        const index = this.children.indexOf(child);
        if (index !== -1) {
            child.parent = null;
            this.children.splice(index, 1);
            return true;
        }
        return false;
    }

    // Przyjmuje wizytatora (metoda wzorca Visitor)
    public accept(visitor: ITreeVisitor<T>, depth: number = 0): void {
        visitor.visit(this, depth);
        for (const child of this.children) {
            child.accept(visitor, depth + 1);
        }
    }
}

// 2. Klasa drzewa zarządzająca korzeniem (Root)
export class Tree<T> {
    public root: TreeNode<T> | null;

    constructor(rootValue?: T) {
        this.root = rootValue !== undefined ? new TreeNode<T>(rootValue) : null;
    }

    // Realizacja wzorca Visitor na całym drzewie
    public accept(visitor: ITreeVisitor<T>): void {
        if (this.root) {
            this.root.accept(visitor, 0);
        }
    }

    // Przechodzenie w głąb (Depth-First Search - DFS) z przekazaniem funkcji zwrotnej (akcji)
    public traverseDFS(action: (value: T, depth: number) => void): void {
        const traverse = (node: TreeNode<T>, depth: number) => {
            action(node.value, depth);
            for (const child of node.children) {
                traverse(child, depth + 1);
            }
        };

        if (this.root) {
            traverse(this.root, 0);
        }
    }

    // Przechodzenie wszerz (Breadth-First Search - BFS) z kolejką FIFO
    public traverseBFS(action: (value: T) => void): void {
        if (!this.root) return;

        const queue: TreeNode<T>[] = [this.root];

        while (queue.length > 0) {
            const current = queue.shift()!;
            action(current.value);
            for (const child of current.children) {
                queue.push(child);
            }
        }
    }

    // Wyszukiwanie węzła spełniającego podany warunek
    public find(predicate: (value: T) => boolean): TreeNode<T> | null {
        if (!this.root) return null;

        const queue: TreeNode<T>[] = [this.root];
        while (queue.length > 0) {
            const current = queue.shift()!;
            if (predicate(current.value)) {
                return current;
            }
            for (const child of current.children) {
                queue.push(child);
            }
        }
        return null;
    }
}

// Wizytator wypisujący strukturę hierarchii z wcięciami w konsoli
export class ConsolePrintVisitor<T> implements ITreeVisitor<T> {
    private _formatter: (val: T) => string;

    constructor(formatter: (val: T) => string = (val) => String(val)) {
        this._formatter = formatter;
    }

    visit(node: TreeNode<T>, depth: number): void {
        const indent = "  ".repeat(depth);
        const prefix = node.isLeaf() ? "└── " : "├── ";
        console.log(`${indent}${prefix}${this._formatter(node.value)}`);
    }
}

// Wizytator zliczający całkowitą liczbę węzłów i liści w drzewie
export class StatisticsVisitor<T> implements ITreeVisitor<T> {
    public totalNodes: number = 0;
    public leafCount: number = 0;

    visit(node: TreeNode<T>): void {
        this.totalNodes++;
        if (node.isLeaf()) {
            this.leafCount++;
        }
    }
}