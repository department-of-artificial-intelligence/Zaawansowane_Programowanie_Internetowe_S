export function measureTime<This, Args extends unknown[], Return>(
    target: (this: This, ...args: Args) => Return,
    context: ClassMethodDecoratorContext<This, (this: This, ...args: Args) => Return>
) {
    return function (this: This, ...args: Args): Return {
        const start = performance.now();
        const result = target.call(this, ...args);
        console.log(`${String(context.name)} took ${(performance.now() - start).toFixed(3)} ms`);
        return result;
    };
}
