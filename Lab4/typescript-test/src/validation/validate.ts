export type ValidationRule = {
    validator: (value: unknown) => boolean;
    message: string;
};

/**
 * Accessor decorator factory - wraps the setter of a property
 * and checks every rule before the value is assigned.
 * Because the constructor assigns values through the setters,
 * it is impossible to create an object that breaks the rules.
 */
export function validate(...rules: ValidationRule[]) {
    return function (_target: object, propertyKey: string | symbol, descriptor: PropertyDescriptor): PropertyDescriptor {
        const originalSet = descriptor.set;
        if (!originalSet) {
            throw new Error(`@validate can only be used on accessors with a setter (${String(propertyKey)})`);
        }
        descriptor.set = function (this: unknown, value: unknown) {
            for (const rule of rules) {
                if (!rule.validator(value)) {
                    throw new Error(rule.message);
                }
            }
            originalSet.call(this, value);
        };
        return descriptor;
    };
}
