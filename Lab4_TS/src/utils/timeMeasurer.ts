export function measureTime<TThis, TArgs extends unknown[], TReturn>(
    target: (this: TThis, ...args: TArgs) => TReturn,
    context: ClassMethodDecoratorContext<TThis, (this: TThis, ...args: TArgs) => TReturn>
){
    const methodName = String(context.name);

    return function(this: TThis, ...args: TArgs): TReturn{
        const start = performance.now();
        const result = target.apply(this, args);
        const end = performance.now();
        console.log(`[${methodName}] executed in - ${end - start}ms`);
        return result;
    };
}
