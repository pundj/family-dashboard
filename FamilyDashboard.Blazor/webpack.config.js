const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const dirName = 'wwwroot/dist';

module.exports = {
    entry: {
        index: './src/js/index'
    },
    plugins: [
        new CleanWebpackPlugin(),
        new MiniCssExtractPlugin({ filename: 'css/site.css' }),
    ],
    experiments: {
        outputModule: true,
    },
    output: {
        filename: 'js/[name].js',
        path: path.resolve(__dirname, dirName),
        library: {
            type: 'module',
        },
        assetModuleFilename: 'assets/[hash][ext][query]',
    },
    module: {
        rules: [
            {
                test: /\.woff($|\?)|\.woff2($|\?)|\.ttf($|\?)|\.eot($|\?)|\.svg($|\?)/i,
                type: 'asset/resource',
                generator: {
                    filename: 'fonts/[name][ext][query]',
                },
            },
            {
                test: /\.(css|s[c|a]ss)$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    {
                        loader: 'postcss-loader',
                        options: {
                            postcssOptions: {
                                plugins: [['postcss-preset-env', {}]],
                            },
                        },
                    },
                    'sass-loader',
                ],
            }
        ]
    }
};