module.exports = function (context, req) {
    context.log('JS HTTP function processed a request: ' + req.body.text);

    context.log(context);
    //context.bindings.redisItem.textValue = 'setting value from javascript';
    context.done();
};