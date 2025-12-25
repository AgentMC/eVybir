// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function editCandidate(key, name, date, description, kind) {
    document.getElementById('candId').value = key;
    document.getElementById('candName').value = name;
    document.getElementById('candDate').value = date;
    document.getElementById('candDesc').value = description;
    document.getElementById('candKind').value = kind;
}
function editCampaign(key, name, dateStart, dateEnd) {
    document.getElementById('camId').value = key;
    document.getElementById('camName').value = name;
    document.getElementById('startDate').value = dateStart;
    document.getElementById('endDate').value = dateEnd;
}
function switchCampaign() {
    if (confirm("Зміни на цій сторінці не були збережені!\r\nПерейти на іншу виборчу подію?")) {
        var selectedCampaignId = document.getElementById('topCampaignSelector').value;
        location.href = selectedCampaignId;
    }
}
function filterCandidates() {
    var filter = $('input[type=radio]:checked').attr('data-filter');
    $('#leftSelect option').each((_, o) => {
        o = $(o);
        if (filter.includes(o.attr('data-kind'))) {
            o.show();
        } else {
            o.hide();
        }
    });
}

function _beginOpCore(parseModel = true) {
    var hidden = $('#inclusionModel')[0];
    return {
        "LeftSelect": $('#leftSelect')[0],
        "RightSelect": $('#rightSelect')[0],
        "Hidden": hidden,
        "Model": parseModel ? JSON.parse(hidden.value) : undefined
    }
}

function _getOptionByValue(select, value) {
    return Array.from(select.options).find(o => o.value == value);
}

function _getNextSortedId(leftSelect, cIdString) {
    for (var i = document.sortedIds.indexOf(Number.parseInt(cIdString)) + 1; i < document.sortedIds.length; i++) {
        var nextCId = document.sortedIds[i];
        var option = _getOptionByValue(leftSelect, nextCId);
        if (option != undefined) return option;
    }
    return undefined;
}

function _getParticipantLocation(model, cId) {
    for (var i = 0; i < model.length; i++) {
        if (model[i].CandidateId == cId) {
            return {
                "Parent": i,
                "Child": -1
            }
        }
        for (var j = 0; j < model[i].Children.length; j++) {
            if (model[i].Children[j].CandidateId == cId) {
                return {
                    "Parent": i,
                    "Child": j
                }
            }
        }
    }
    return undefined;
}

///Returns an object describing the location to add next item:
///{Parent: int?, Child: int?, Option: <option>?}
///Parent and Child are Model indices as per _getParticipantLocation()
///Option is the DOM <option> object before which to insert - or null (append)
///In all 3 cases, undefined means "no recommendation, append" (it works for "Select.options.add()").
///For Ints, -1 on Child means "inapplicable as the item is Parent".
function _getNextOption(rightSelect, isParent, model) {
    //An item must be selected on the right
    if (rightSelect.selectedIndex == -1) return { }; //all undefined, append

    var location = _getParticipantLocation(model, rightSelect.selectedOptions[0].value);
    
    //Add Child when a child is selected --> insert before the one selected
    if (location.Child > -1 && !isParent) {
        location.Option = rightSelect.selectedOptions[0];
        return location;
    }

    //Add Child when Parent is selected --> model: append the Children | ux: insert before next Parent or Append if last
    //Add Parent when Parent is selected --> both: insert Parent before the next Parent or Append if last
    //Add Parent when Child is selected --> same as above; we ignore the fact that a child is selected and just work on it's parent
    var uxBeforeParentModelIndex = location.Parent + 1;
    location.Option = uxBeforeParentModelIndex < model.length //is the next Parent available?
        ? _getOptionByValue(rightSelect, model[uxBeforeParentModelIndex].CandidateId)
        : undefined;
    if (!isParent) { //adding child
        location.Child = undefined; //append to Children collection
    } else {
        location.Parent = location.Option != undefined ? uxBeforeParentModelIndex : undefined; //if next Parent is available, insert before it else append
    }
    return location;
}

///See _getNextOption(...)
///1. This one pre-processes the undefined on Ints so that the data can be passed onto splice() over model
///2. This one adds CandidateId property which is int = value of the <option> being moved:
///{Parent: int?, Child: int?, Option: <option>?, CandidateId: int}
function _beginUxMove(o, isParent) {
    //locate
    var data = _getNextOption(o.RightSelect, isParent, o.Model);
    //pre-process and validate data
    if (data.Parent != undefined && data.Child == undefined) data.Child = o.Model[data.Parent].Children.length;
    if (data.Parent == undefined && isParent) data.Parent = o.Model.length;
    if (data.Parent == undefined && !isParent) throw "Attempting to add a Child but _getNextOption() could not resolve Parent.";
    //ux movement, fixing the selected parent
    var option = o.LeftSelect.selectedOptions[0];
    var selectedRightOption = o.RightSelect.selectedOptions[0];
    o.RightSelect.options.add(option, data.Option);
    $(option).addClass(isParent ? 'node-primary' : 'node-secondary');
    if (selectedRightOption != undefined) selectedRightOption.selected = true;
    //CandidateId
    data.CandidateId = Number.parseInt(option.value);
    //return
    return data;
}

function updateButtonStates() {
    var o = _beginOpCore();
    var lsi = o.LeftSelect.selectedIndex;
    var rsi = o.RightSelect.selectedIndex;
    var rLocation = {};
    var rIsChild = false;
    var rKind = undefined;
    if (rsi > -1) {
        rLocation = _getParticipantLocation(o.Model, o.RightSelect.value);
        rIsChild = rLocation.Child > -1;
        rKind = o.RightSelect.selectedOptions[0].getAttribute('data-kind');
        //^--Spoil|Person|Group
    }

    //Add Parent
    if (lsi > -1) {
        $('#btnAddParent').removeAttr('disabled');
    } else {
        $('#btnAddParent').attr('disabled', true);
    }

    //Add Child
    var enableAddChild = false;
    if (lsi > -1 && rsi > -1 && (rIsChild || rKind == 'Group')) {
        enableAddChild = true;
        for (var i = 0; i < o.LeftSelect.selectedOptions.length; i++) {
            if (o.LeftSelect.selectedOptions[i].getAttribute('data-kind') != 'Person') {
                enableAddChild = false;
            }
        }
    }
    if (enableAddChild) {
        $('#btnAddChild').removeAttr('disabled');
    } else {
        $('#btnAddChild').attr('disabled', true);
    }

    //Remove selected
    if (rsi > -1) {
        $('#btnDel').removeAttr('disabled');
    } else {
        $('#btnDel').attr('disabled', true);
    }

}

function _beginOp(parseModel = true) {
    var o = _beginOpCore(parseModel);
    o.Finalize = function () {
        this.Hidden.value = this.Model == undefined ? '[]' : JSON.stringify(this.Model);
        updateButtonStates();
    }
    return o;
}

function addAsParent() {
    var o = _beginOp();
    var nodeCount = o.LeftSelect.selectedOptions.length;
    for (var i = 0; i < nodeCount; i++) {
        //locate and update UX
        var insertData = _beginUxMove(o, true);
        //model
        o.Model.splice(insertData.Parent, 0, {
            "CandidateId": insertData.CandidateId,
            "Children": []
        });
    }
    o.Finalize();
}

function addAsChild() {
    var o = _beginOp();
    var nodeCount = o.LeftSelect.selectedOptions.length;
    for (var i = 0; i < nodeCount; i++) {
        //locate and update UX
        var insertData = _beginUxMove(o, false);
        //model
        var parent = o.Model[insertData.Parent];
        parent.Children.splice(insertData.Child, 0, {
            "CandidateId": insertData.CandidateId,
            "GroupId": parent.CandidateId,
            "Children": []
        });
    }
    o.Finalize();
}

function removeFromList() {
    var o = _beginOp();
    var option = o.RightSelect.selectedOptions[0];
    var location = _getParticipantLocation(o.Model, option.value);
    if (location == undefined) {
        alert("Unable to get model location for the selected participant's id!");
        return;
    }
    o.LeftSelect.options.add(option, _getNextSortedId(o.LeftSelect, option.value));
    if (location.Child == -1) { //parent
        o.Model.splice(location.Parent, 1);
        $(option).removeClass('node-primary')
    } else { //child
        o.Model[location.Parent].Children.splice(location.Child, 1);
        $(option).removeClass('node-secondary')
    }
    o.Finalize();
}

function removeAll() {
    var o = _beginOp(false);
    for (var i = o.RightSelect.options.length - 1; i >= 0; i--) {
        var option = o.RightSelect.options[i];
        o.LeftSelect.options.add(option, _getNextSortedId(o.LeftSelect, option.value));
        $(option).removeClass('node-primary').removeClass('node-secondary');
    }
    o.Finalize();
}

function setOrder() {
    function setOrderInternal(list) {
        for (var i = 0; i < list.length; i++) {
            list[i].DisplayOrder = i;
            setOrderInternal(list[i].Children);
        }
    }
    var o = _beginOp();
    setOrderInternal(o.Model);
    o.Finalize();
}