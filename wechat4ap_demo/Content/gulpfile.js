var gulp = require('gulp');
var gulpIf = require('gulp-if');
var gulpConcat = require('gulp-concat');
var gulpDel = require('del');

var gulpValidateJs = require('gulp-eslint');
var gulpSourceMaps = require('gulp-sourcemaps');
var gulpMinifyJs = require('gulp-uglify');

var gulpValidateCss = require('gulp-css-validator');
var gulpMinifyCss = require('gulp-minify-css');

var gulpValidateHtml = require('gulp-htmlhint');
var gulpHtmlReplace = require('gulp-html-replace');
var gulpMinifyHtml = require('gulp-minify-html');

var gulpTemplateCache = require('gulp-angular-templatecache');

gulp.task('default', ['javascript', 'css', 'font']);

gulp.task('javascript', function () {
	gulpDel('dist/js/**/*.js').then(
	gulp.src([
        'bower_components/jquery/dist/jquery.js',
		'bower_components/angular/angular.js',
		'bower_components/angular-bootstrap/ui-bootstrap-tpls.js',
        'bower_components/angular-moment/angular-moment.js',
        'bower_components/angular-animate/angular-animate.js',
        'bower_components/bootstrap/dist/js/bootstrap.js',
        'src/app/app.js'])
		.pipe(gulpSourceMaps.init())
		.pipe(gulpIf('!min\.js', gulpMinifyJs()))
		.pipe(gulpConcat('app.min.js'))
		.pipe(gulpSourceMaps.write('../maps'))
		.pipe(gulp.dest('dist/js')));
});

gulp.task('css', function () {
	gulpDel('dist/**/*.css').then(
	gulp.src([
        'bootstrap.css',
		'src/app/**/*.css'])
		.pipe(gulpIf('!min\.css', gulpMinifyCss()))
		.pipe(gulpConcat('app.min.css'))
		.pipe(gulp.dest('dist/css')));
});

gulp.task('font', function () {
    gulpDel([
        'dist/fonts/**/*.eot',
        'dist/fonts/**/*.svg',
        'dist/fonts/**/*.ttf',
        'dist/fonts/**/*.woff']).then(
	gulp.src([
        'bower_components/bootstrap/fonts/**/*.eot',
        'bower_components/bootstrap/fonts/**/*.svg',
        'bower_components/bootstrap/fonts/**/*.ttf',
        'bower_components/bootstrap/fonts/**/*.woff'])
		.pipe(gulp.dest('dist/fonts')));
});

