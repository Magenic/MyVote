var sharedConfig = require('./controllers_shared.conf.js');

module.exports = function(config) {
    sharedConfig(config);

    config.set({
        browsers: ['PhantomJS'],
        singleRun: true
    });
};