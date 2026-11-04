import type { ValidationRule } from "./validate";

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function required(message = "To pole jest wymagane"): ValidationRule {
    return {
        validator: value => value !== null && value !== undefined
            && (typeof value !== "string" || value.trim().length > 0),
        message
    };
}

export function isEmail(message = "Niepoprawny adres e-mail"): ValidationRule {
    return {
        validator: value => typeof value === "string" && EMAIL_REGEX.test(value),
        message
    };
}
