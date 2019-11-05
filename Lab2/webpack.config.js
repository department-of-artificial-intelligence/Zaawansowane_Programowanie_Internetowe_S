var htmlWebpackPlugin = require("html-webpack-plugin");
var path = require("path");
var bodyParser = require('body-parser');

module.exports = {
    mode: "development",
    devtool: "source-map",
    entry: {app:"./src/index.ts"},
    output: {
        path: path.join(__dirname,"./build/"),
        filename: "bundle.js"
    },
    watch:true,
    resolve:{
        extensions: [".js",".ts"]
    },
    module:{
        rules:[
            {test:/\.ts$/, loader:"ts-loader"}
        ]
    },
    devServer: {
        contentBase: path.join(__dirname,'./build/'),
        port:9000,
        before:function (app) {
            let i=0;
            app.use(bodyParser.json());

            app.all('/przetworzDane', bodyParser.json(), (req, res)=>{
                res.send(req.body);
            });

            app.all('/error', (req, res)=>{
                res.statusCode = 404;
                res.send();
            });

            app.all('/error1', bodyParser.json(), (req, res)=>{
                i++;
                console.log(i);
                if(i%3==0)
                    res.statusCode = 200;
                else
                    res.statusCode = 500;
                res.send(req.body);
            });
        }
    },
    plugins:[
        new htmlWebpackPlugin({
            hash:true,
            title:"Nasza pierwsza aplikacja typescript",
            myPageHeader: 'Nasza pierwsza aplikacja typescript',
            template: './build/index.html',
            chunks: ['vendor','shared','app'],
            path: path.join(__dirname, "./build/"),
            filename:'index.html' 
        })
    ]
}