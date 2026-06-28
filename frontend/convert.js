module.exports = function(fileInfo, api) {
  const j = api.jscodeshift;
  const root = j(fileInfo.source);

  root.find(j.FunctionDeclaration).forEach(path => {
    let body = path.value.body; // BlockStatement
    let isExpression = false;

    // Check for implicit return (single return statement)
    if (body.body && body.body.length === 1 && body.body[0].type === 'ReturnStatement') {
      body = body.body[0].argument;
      isExpression = true;
    }

    const arrowFunc = j.arrowFunctionExpression(
      path.value.params,
      body,
      isExpression
    );
    arrowFunc.async = path.value.async;
    
    // Keep TS types
    if (path.value.returnType) arrowFunc.returnType = path.value.returnType;
    if (path.value.typeParameters) arrowFunc.typeParameters = path.value.typeParameters;

    const varDecl = j.variableDeclaration('const', [
      j.variableDeclarator(path.value.id, arrowFunc)
    ]);

    const parent = path.parentPath;
    if (parent.value.type === 'ExportNamedDeclaration') {
      parent.replace(j.exportNamedDeclaration(varDecl));
    } else if (parent.value.type === 'ExportDefaultDeclaration') {
      parent.replace(varDecl);
      parent.insertAfter(j.exportDefaultDeclaration(path.value.id));
    } else {
      path.replace(varDecl);
    }
  });

  return root.toSource();
};
module.exports.parser = 'tsx';
