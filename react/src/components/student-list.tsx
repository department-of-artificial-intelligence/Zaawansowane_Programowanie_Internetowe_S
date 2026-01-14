import { ChangeEvent, FormEvent, useState } from "react";
import { Student } from "../model/student";
import { Course } from "../model/course";

const students: Student[] = [new Student("Dorota", "Test", new Course("Informatyka")), new Student("Agnieszka", "Test", new Course("Budownictwo"))];

type CreateStudentState {
    firstName: string,
    lastName: string,
    course: Course,
}

type CreateStudentProperties {
    studentCreated: (student: Student) => void;
    canceled: () => void;
}


export const CreateStudent = ({ studentCreated, canceled }: CreateStudentProperties) => {
    const [student, setStudent] = useState({
        firstName: '',
        lastName: '',
        course: new Course('')
    } as CreateStudentState);

    const change = (e: ChangeEvent<HTMLInputElement>) => {
        let { name, value } = (e.target as HTMLInputElement);
        let newState: CreateStudentState = { ...student, [name]: value };
        setStudent(newState);
    }

    //const submitHandler = (e:SelectChangeEvent)
}

export const StudentList = () => {
    const [listShown, setListShown] = useState({
        isListShown: false,
        title: 'Pokaż listę'
    });

    const [displayedStudents, setDisplayedStudents] = useState([...students]);

    const changeListVisibility = () => {
        const newState = {
            title: listShown.isListShown ? "Pokaż listę" : "Ukryj listę",
            isListShown: !listShown.isListShown,
        }
        setListShown(newState);
    }

    return (
        <>
            <button onClick={changeListVisibility}>{listShown.title}</button>
            {
                listShown.isListShown && <ol> {
                    displayedStudents.map((s: Student, idx: number) =>

                        <li>
                            {s.firstName} {s.lastName} {s.course.name};
                        </li>
                    )
                }
                </ol>

            }
        </>
    );
}