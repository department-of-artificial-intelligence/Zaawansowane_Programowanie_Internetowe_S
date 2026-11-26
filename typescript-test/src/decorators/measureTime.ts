export function measureTime<This, Args extends unknown[], Return>(
  target: (this: This, ...args: Args) => Return,
  context: ClassMethodDecoratorContext<
    This,
    (this: This, ...args: Args) => Return
  >
): (this: This, ...args: Args) => Return {
  const methodName = String(context.name);

  return function (this: This, ...args: Args): Return {
    const start = performance.now();
    const result = target.apply(this, args);

    const end = performance.now();
    console.log(
      `[measureTime] Metoda "${methodName}" wykonała się w ${(
        end - start
      ).toFixed(4)} ms`
    );
    return result;
  };
}
