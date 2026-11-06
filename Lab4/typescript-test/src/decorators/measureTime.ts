export function measureTime<This, Args extends any[], Return>(
    target: (this: This, ...args: Args) => Return,
    context: ClassMethodDecoratorContext<This, (this: This, ...args: Args) => Return>
) {
    const methodName = String(context.name);

    return function (this: This, ...args: Args): Return {
        const start = performance.now();

        // Wywołanie oryginalnej metody z zachowaniem kontekstu this i argumentów
        const result = target.apply(this, args);

        const end = performance.now();
        const duration = (end - start).toFixed(3);

        console.log(`⏱️ [measureTime] Metoda '${methodName}' wykonała się w ${duration} ms.`);

        // Zwrócenie nienaruszonego wyniku metody
        return result;
    };
}