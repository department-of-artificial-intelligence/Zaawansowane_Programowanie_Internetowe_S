var tabela = [1,2,3,4,5,6,7,8];

function wiekszeOd(y) {
    return (x)=>x>y;
}

tabela.filter(wiekszeOd(3)).reduce((a,x)=>a+x);