var tabela_ = ["1","2","3","Ala","5","Kot","7","8"];

tabela_.map(x=>parseInt(x)).filter(x=>!isNaN(x)).reduce((a,x)=>a+x);