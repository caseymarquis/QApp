const path = require('path');
const chalk = require('chalk');
const webpack = require('webpack');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const TerserPlugin = require('terser-webpack-plugin');

//Figure out our build mode:
var isDev = false;
var isDevServer = false;
process.argv.forEach(function (val, index, array) {
    console.log(index + " : " + val);
    if (val === "--env.build=dev") {
        isDev = true;
    }
    else if (val === "--env.isDevServer=true") {
        isDevServer = true;
    }
});

//Try to prevent mistaken builds:
if (isDev) {
    console.log(chalk.red("Build Mode == DEV"));
}
else {
    console.log(chalk.red("Build Mode == PROD"));
}

if (isDevServer) {
    console.log(chalk.red("webpack-dev-server detected"));
}

//Set mode specific items:
var outputDirectory = isDevServer ? './app/_dev-server' : '../wwwroot';

//Main config:
module.exports =
    {
        context: path.join(__dirname, 'app'),
        entry: {
            app: './main.js',
        },
        output: {
            globalObject: 'self',
            path: path.join(__dirname, outputDirectory),
            filename: 'app.bundle.[chunkhash].js'
        },
        mode: isDev? 'development' : 'production',
        devtool: 'source-map',
        optimization: {
            splitChunks: {
                chunks: 'async',
                minSize: 30000,
                maxSize: 0,
                minChunks: 1,
                maxAsyncRequests: 6,
                maxInitialRequests: 4,
                automaticNameDelimiter: '~',
                automaticNameMaxLength: 30,
                cacheGroups: {
                    defaultVendors: {
                        test: /[\\/]node_modules[\\/]/,
                        priority: -10
                    },
                    default: {
                        minChunks: 2,
                        priority: -20,
                        reuseExistingChunk: true
                    }
                }
            },
            minimize: !isDev,
            minimizer: [
                new TerserPlugin({
                    chunkFilter: (chunk) => {
                        // Exclude uglification for the `vendor` chunk
                        let name = chunk.name;
                        if (name && name.includes('vendor')) {
                            return false;
                        }
                        return true;
                    },
                }),
            ],
        },
        module: {
            rules: [
                { test: /\.(scss)$/, use: [
                    { loader: "vue-style-loader"},
                    { loader: 'css-loader' },
                    {
                        loader: 'postcss-loader',
                        options: {
                            postcssOptions: {
                                plugins: [
                                    ['precss'],
                                    ['autoprefixer']
                                ]
                            }
                        }
                    },
                    { 
                        loader: 'sass-loader',
                        options: {
                            sassOptions: {
                                includePaths: [path.resolve(__dirname, "./app/scss")],
                            }
                        }
                    }]
                },
                { test: /\.css$/, use: [
                    { loader: "vue-style-loader"},
                    { loader: "css-loader"}]
                },
                { test: /\.(woff|woff2)(\?v=\d+\.\d+\.\d+)?$/, use: [{
                        loader: 'url-loader',
                        options: {
                            limit: 10000,
                            mimetype: 'application/font-woff'
                        }
                    }]
                },
                { test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/, use: [{
                        loader: 'url-loader',
                        options: {
                            limit: 10000,
                            mimetype: 'application/octet-stream'
                        }
                    }]
                },
                { test: /\.eot(\?v=\d+\.\d+\.\d+)?$/, use: 'file-loader' },
                { test: /\.svg(\?v=\d+\.\d+\.\d+)?$/, use: [{
                        loader: 'url-loader',
                        options: {
                            limit: 10000,
                            mimetype: 'application/svg+xml'
                        }
                    }]
                },
                { test: /\.png$/, use: [{
                        loader: 'url-loader',
                        options: {
                            limit: 10000
                        }
                    }]
                },
                { test: /\.jpg$/, use: "file-loader" },
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: [{
                        loader: 'babel-loader',
                        options: {presets: ['@babel/preset-env']}
                    }]
                },
                { test: /\.vue$/, use: "vue-loader" },
            ]
        },
        plugins: [
            new VueLoaderPlugin(),
            new HtmlWebpackPlugin({
                hash: true,
                template: './index.html',
                filename: 'index.html',
            }),
            //new BundleAnalyzerPlugin(), //Uncomment to view analysis of bundle size.
            new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
        ],
    };