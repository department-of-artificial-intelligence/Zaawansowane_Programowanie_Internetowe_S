const HtmlWebpackPlugin = require("html-webpack-plugin");
const path = require("path");

module.exports = {
    mode: "development",
    devtool: "source-map",
    entry: {
        app: "./src/index.ts"
    },
    output: {
        path: path.join(__dirname, "./build"),
        filename: "bundle.js"
    },
    resolve: { extensions: [".js", ".ts"] },
    devServer: {
        static: path.join(__dirname, "./build/"),
        port: 9000
    },
    module: {
        rules: [
            { test: /\.ts$/, loader: "ts-loader" }
        ]
    },
    plugins: [
        new HtmlWebpackPlugin({
            hash: true,
            title: "Nasza pierwsza aplikacja TypeScript",
            template: "./build/index.html",
            filename: "index.html"
        })
    ]
};
