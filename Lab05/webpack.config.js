var htmlWebpackPlugin = require("html-webpack-plugin");
var path = require("path");
var bodyParser = require('body-parser');

module.exports = {
    devtool: "source-map",
    entry: { myapp: "./src/index.ts" },
    output: {
        path: path.join(__dirname, "./build/"),
        filename: "bundle.js"
    },
    mode: 'development',
    devServer: {
        static: path.join(__dirname, './build/'),
        port: 7000,
        setupMiddlewares: function (middlewares, devServer) {
            let i = 0;
            if (!devServer) {
                throw new Error('webpack-dev-server is not defined');
            }

            devServer.app.use(bodyParser.json());

            devServer.app.all('/proceed', bodyParser.json(), (req, res) => {
                res.send(req.body);
            });

            devServer.app.all('/error', (req, res) => {
                res.statusCode = 404;
                res.send();
            });

            devServer.app.all('/error1', bodyParser.json(), (req, res) => {
                i++;
                if (i % 3 == 0)
                    res.statusCode = 200;
                else
                    res.statusCode = 500;
                res.send(req.body);
            });

            return middlewares;
        }
    },
    resolve: {
        extensions: [".js", ".ts"]
    },
    module: {
        rules: [
            { test: /\.ts$/, loader: "ts-loader" }
        ],
    },
    plugins: [
        new htmlWebpackPlugin({
            hash: true,
            template: './src/index.html',
            path: path.join(__dirname, "./build/"),
            filename: 'index.html'
        })
    ]
}