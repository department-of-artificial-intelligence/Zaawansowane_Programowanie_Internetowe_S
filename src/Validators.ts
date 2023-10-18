export function required(target: object, name: string) {
    Object.defineProperty(target, name, {
        get: function () { return this[`__${name}`]; },
        set: function (value?: string) {
            if (value === null || value === undefined || value.length === 0) {
                throw new Error(`${name} nie może być puste`);
            }
            this[`__${name}`] = value;
        }
    });
}


export function validateEmail(email:string):boolean {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return email.match(emailRegex) !== null;
}


export function validate<T>(
    conditions: { validator: (param: T) => boolean, message: string }[]
    ) : PropertyDecorator
    {
    return function (target: object, name: string | symbol) {
        Object.defineProperty(target, name, {
        get: function () { return this['__${name.toString()}']; },
        set: function (value: T) {
            for (let condition of conditions) {
                if (!condition.validator(value))
                    throw new Error(condition.message);
                }
            this['__${name.toString()}'] = value;
            }
        })
    }
}