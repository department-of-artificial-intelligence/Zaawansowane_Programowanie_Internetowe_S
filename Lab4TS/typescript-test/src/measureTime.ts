export function measureTime( _target: object, propertyKey: string, descriptor: PropertyDescriptor): PropertyDescriptor {
  const originalMethod = descriptor.value;

  descriptor.value = function (...args: unknown[]) {
    const start = performance.now();
    try {
      return originalMethod.apply(this, args);
    } finally {
      const end = performance.now();
      console.log(
        `[measureTime] Metoda "${propertyKey}" wykonała się w ${(end - start).toFixed(3)} ms`
      );
    }
  };

  return descriptor;
}

class KalkulatorService {
  @measureTime
  public obliczSume(limit: number): number {
    let suma = 0;
    for (let i = 0; i < limit; i++) {
      suma += i;
    }
    return suma;
  }
}

const serwis = new KalkulatorService();
const wynik = serwis.obliczSume(1_000_000);
console.log('Zwrócona wartość:', wynik);