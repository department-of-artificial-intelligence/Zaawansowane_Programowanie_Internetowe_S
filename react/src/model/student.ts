import { Course } from './course';

export class Student {
    _id: number | undefined;
    _firstName: string;
    _lastName: string;
    _course: Course;

    constructor(firstName: string, lastName: string, course: Course) {
        this._firstName = firstName;
        this._lastName = lastName;
        this._course = course;
    }

    public get firstName() {
        return this._firstName;
    }

    public get lastName() {
        return this._lastName;
    }

    public get course() {
        return this._course;
    }

    public get id() {
        return this._id;
    }

    public set id(id: number | undefined) {
        this._id = id;
    }
}