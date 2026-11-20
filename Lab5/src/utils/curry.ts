export function curry2<T1, T2, ReturnT>(fn:(arg1:T1, arg2:T2)=>ReturnT) {
    return (a1:T1)=>(a2:T2)=>fn(a1,a2);
}