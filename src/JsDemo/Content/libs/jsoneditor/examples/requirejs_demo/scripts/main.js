require.config({
    waitSeconds: 2000,
    paths: {
        'jsoneditor': '/Content/libs/jsoneditor/dist/jsoneditor'
    }
});
require(['jsoneditor'], function (JsonEditor) {
    
    var options = {
        mode: 'tree',
        modes: ['code', 'form', 'text', 'tree', 'view'], // allowed modes
        onError: function (err) {
            alert(err.toString());
        },
        onModeChange: function (newMode, oldMode) {
            console.log('Mode switched from', oldMode, 'to', newMode);
        }
    };
    // create the editor
    var container = document.getElementById('jsoneditor');

    var editor = new JsonEditor(container, options);
    //var editor = new JsonEditor(container);



  // set json
  document.getElementById('setJSON').onclick = function () {
    var json = {
      'array': [1, 2, 3],
      'boolean': true,
      'null': null,
      'number': 123,
      'object': {'a': 'b', 'c': 'd'},
      'string': 'Hello World'
    };
    editor.set(json);
  };

  // get json
  document.getElementById('getJSON').onclick = function () {
    var json = editor.get();
    alert(JSON.stringify(json, null, 2));
  };
});
