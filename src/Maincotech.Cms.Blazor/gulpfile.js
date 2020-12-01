/// <binding BeforeBuild='default' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp'),
    cleanCss = require('gulp-clean-css'),
    less = require('gulp-less'), 
    concat = require("gulp-concat"),
    npmImport = require("less-plugin-npm-import"),
    autoprefixer = require("gulp-autoprefixer");

gulp.task('less', function () {
    return gulp
        .src([
            '**/*.less',
            '!node_modules/**'
        ])
        .pipe(less({
            javascriptEnabled: true,
            plugins: [new npmImport({ prefix: '~' })]
        }))
        .pipe(concat('component.css'))
        .pipe(autoprefixer("last 2 version", "ie11", "safari 10"))
        .pipe(cleanCss({ compatibility: '*' }))
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('default', gulp.parallel('less'), function () { })