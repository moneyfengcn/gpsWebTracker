var App = function () {


    //地图对象
    var _map = null;

    //车辆图标列表
    var _lstVehicles = new Array();

    //跟踪目标
    var watch_devceId = '';

    //-------------------------------------------------------

    //初始化地图
    var _initMap = function () {
        _lstVehicles = new Array();

        var pos = new AMap.LngLat(112, 24);//标准写法

        _map = new AMap.Map('mapContainer', {
            resizeEnable: true, //是否监控地图容器尺寸变化
            zoom: 8,//级别
            center: pos,//中心点坐标
            //  viewMode: '3D'//使用3D视图
        });
        //异步加载插件
        AMap.plugin(['AMap.ToolBar', 'AMap.Scale'], function () {
            var toolbar = new AMap.ToolBar();
            var scale = new AMap.Scale();

            _map.addControl(toolbar);
            _map.addControl(scale);
        });
        _loadMapDefaultView();
    }

    var _setMapView = function (obj) {
        var pos = new AMap.LngLat(obj.lng, obj.lat);//标准写法
        _map.setZoomAndCenter(obj.zoom, pos);
    }

    var _showMessage = function (msg) {
        alert(msg);
    }

    var _getMapMarker = function (id) {
        if (_lstVehicles.length >= 1) {
            for (var i = 0; i < _lstVehicles.length; i++) {
                if (_lstVehicles[i].name == id) {
                    console.debug("找到:" + id);
                    return _lstVehicles[i];
                }
            }
        }
        console.debug("找不到:" + id);
        return null;

    }


    var _moveMarker = function (devceId, name, lat, lng, angle, speed, time) {
        var gps = new AMap.LngLat(lng, lat);//标准写法
 
        //转换坐标系
        AMap.convertFrom(gps, 'gps', function (status, result) {      
            if (result.info === 'ok') {             
                var text = '<center>' + name + '<BR/>速度:' + speed + '<BR/>' + time + '</center>'
                var pos = result.locations[0]; // Array.<LngLat>
                var marker = _getMapMarker(devceId);
                if (marker == null) {
                    marker = _createMarker(devceId, name, pos, angle, text);
                    _lstVehicles.push(marker);
                } else {
                    marker.setPosition(pos);
                    marker.setAngle(angle);
                    marker.setLabel({
                        offset: new AMap.Pixel(5, 5),  //设置文本标注偏移量
                        content: text, //设置文本标注内容
                        direction: 'bottom' //设置文本标注方位
                    });
                }
                if (watch_devceId == devceId) {
                    if (!_map.getBounds().contains(pos)) {
                        _map.panTo(pos);
                    }
                }
            } else {
                console.log("转换坐标系失败：");
            }
        });
    }

    var _setWatchTarget = function (devceId) {
        watch_devceId = devceId;
    }

    var _createMarker = function (id, name, pos, angle, text) {
        //建立车辆图标
        var marker = new AMap.Marker({
            icon: "/images/car.png",
            position: pos,
            angle: angle,
            title: name
        });
        marker.setLabel({
            offset: new AMap.Pixel(5, 5),  //设置文本标注偏移量
            content: text, //设置文本标注内容
            direction: 'bottom' //设置文本标注方位
        });
        marker.on('dblclick', on_marker_rightclick);
        marker.name = id;
        _map.add(marker);
        return marker;
    }

    function on_marker_rightclick(e) {
        var pos = this.getPosition();

        _map.setZoomAndCenter(15, pos);
        _setWatchTarget(this.name);
        console.info(this);
    }

    var _clearMap = function () {
        _lstVehicles = new Array();
        _map.clearMap();
    }

    var _InitMapContextMenu = function () {
        getSubMenuByDeviceList = function () {
            var items = new Array();
            items[0] = {
                header: '车辆列表'
            };

            if (_lstVehicles.length >= 1) {
                for (var i = 0; i < _lstVehicles.length; i++) {
                    items.push({
                        key: _lstVehicles[i].name,
                        icon: 'glyphicon-edit',
                        text: _lstVehicles[i].getTitle(),
                        action: function (e, selector, key) {
                            var marker = _getMapMarker(key);
                            var pos = marker.getPosition();

                            _map.setZoomAndCenter(15, pos);
                            _setWatchTarget(key);
                            console.info(marker);

                        }
                    });
                }
            }
            return items;
        }
        map_contextMenu = {
            id: 'MAP-MENU',
            data:
                [
                    {
                        icon: 'glyphicon-trash',
                        text: '取消跟踪',
                        key: "k1",
                        action: function (e, selector, key) {
                            _setWatchTarget("");
                        }
                    },
                    {
                        icon: 'glyphicon-plus',
                        text: '跟踪目标车辆',
                        key: "k2",
                        subMenu: [
                            {
                                menu_item_src: getSubMenuByDeviceList
                            }

                        ]
                    },
                    {
                        divider: true
                    },
                    {
                        icon: 'glyphicon-home',
                        text: '设置默认视野',
                        key: "k3",
                        action: function (e, selector, key) {
                            saveMapDefaultView();
                        }
                    }
                ]
        };

        context.init({ preventDoubleContext: false });
        context.attach('#mapContainer', map_contextMenu);
    }

    var _loadMapDefaultView = function () {
        $.get("/api/userprofile/GetMapDefaultView", function (data) {
            if (data.state) {
                _setMapView(data.data);
            }
        });
    }

    function saveMapDefaultView() {
        var center = _map.getCenter();
        var obj = { zoom: _map.getZoom(), lng: center.getLng(), lat: center.getLat() };
        $.ajax({
            url: "/api/userprofile/SaveMapDefaultView",
            data: JSON.stringify(obj),
            contentType: "application/json",
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.state) {
                    swal("提示", "设置地图默认视野成功!", "success");
                }
                else {
                    swal("提示", "设置地图默认视野失败！", "error");
                }

            }
        });
    }

    return {
        InitMap: _initMap,
        SetMapView: _setMapView,
        MoveMarker: _moveMarker,
        ShowMessage: _showMessage,
        ClearMap: _clearMap,
        SetWatchTarget: _setWatchTarget,
        InitMapContextMenu: _InitMapContextMenu
    };
}();