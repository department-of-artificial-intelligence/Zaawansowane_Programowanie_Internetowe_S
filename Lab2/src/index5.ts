var tabela = [1,2,3,4,5,6,7,8];

function wiekszeOd_(x,y) {
    return x>y;
}
function curry2<T1,T2,T3>(f:(x:T1,y:T2)=>T3) {
    return (x:T1)=>(y:T2) =>f(x,y);
}

tabela.filter(curry2(wiekszeOd_)(3)).reduce((a,x)=>a+x);