// src/decorators/measureTime.ts

// 1. Przeci¹¿enie dla klasycznych dekoratorów (experimentalDecorators)
export function measureTime(
    target: any,
    propertyKey: string | symbol,
    descriptor: PropertyDescriptor
): PropertyDescriptor;

// 2. Przeci¹¿enie dla standardu TypeScript 5.0+
export function measureTime<T extends (...args: any[]) => any>(
    target: T,
    context: ClassMethodDecoratorContext
): T;

// Implementacja
export function measureTime(...args: any[]): any {
    if (args.length === 2 && typeof args[1] === "object" && "kind" in args[1]) {
        const [originalMethod, context] = args;
        const methodName = String(context.name);

        return function (this: any, ...methodArgs: any[]) {
            const start = performance.now();
            const result = originalMethod.apply(this, methodArgs);

            if (result instanceof Promise) {
                return result.then((data: any) => {
                    const end = performance.now();
                    console.log(
                        `%c[Czas wykonania] Metoda (async) "${methodName}" wykona³a siê w ${(end - start).toFixed(3)} ms`,
                        "color: #06b6d4; font-weight: bold;"
                    );
                    return data;
                });
            }

            const end = performance.now();
            console.log(
                `%c[Czas wykonania] Metoda "${methodName}" wykona³a siê w ${(end - start).toFixed(3)} ms`,
                "color: #06b6d4; font-weight: bold;"
            );
            return result;
        };
    }

    const [, propertyKey, descriptor] = args;
    const originalMethod = descriptor.value;
    const methodName = String(propertyKey);

    descriptor.value = function (this: any, ...methodArgs: any[]) {
        const start = performance.now();
        const result = originalMethod.apply(this, methodArgs);

        if (result instanceof Promise) {
            return result.then((data: any) => {
                const end = performance.now();
                console.log(
                    `%c[Czas wykonania] Metoda (async) "${methodName}" wykona³a siê w ${(end - start).toFixed(3)} ms`,
                    "color: #06b6d4; font-weight: bold;"
                );
                return data;
            });
        }

        const end = performance.now();
        console.log(
            `%c[Czas wykonania] Metoda "${methodName}" wykona³a siê w ${(end - start).toFixed(3)} ms`,
            "color: #06b6d4; font-weight: bold;"
        );
        return result;
    };

    return descriptor;
}