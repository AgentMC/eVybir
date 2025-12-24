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

function _getNextOption(rightSelect, isParent, model) {
    if (rightSelect.selectedIndex == -1) return undefined;
    var location = _getParticipantLocation(model, rightSelect.selectedOptions[0].value);
    if (location.Child > -1 && !isParent) return rightSelect.selectedOptions[0];
    var i = location.Parent + 1;
    return i < model.length ? _getOptionByValue(rightSelect, model[i].CandidateId) : undefined;
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
    if (lsi > -1 && rsi > -1 && (rIsChild || rKind == 'Group')) {
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
        var option = o.LeftSelect.selectedOptions[0];
        var beforeOption = _getNextOption(o.RightSelect, true, o.Model);
        o.RightSelect.options.add(option, beforeOption);
        o.Model.push({
            "CandidateId": Number.parseInt(option.value),
            "Children": []
        });
        $(option).addClass('node-primary');
    }
    o.Finalize();
}
function addAsChild() {
    var o = _beginOp();
    var nodeCount = o.LeftSelect.selectedOptions.length;
    for (var i = 0; i < nodeCount; i++) {
        var option = o.LeftSelect.selectedOptions[0];
        var beforeOption = _getNextOption(o.RightSelect, false, o.Model);
        var parent;
        if (beforeOption == undefined) {
            parent = o.Model[o.Model.length - 1];
        } else {
            var beforeOptionLocation = _getParticipantLocation(o.Model, beforeOption.value);
            parent = beforeOptionLocation.Child > -1
                ? o.Model[beforeOptionLocation.Parent]
                : o.Model[beforeOptionLocation.Parent - 1];
        }
        o.RightSelect.options.add(option, beforeOption);
        parent.Children.push({
            "CandidateId": Number.parseInt(option.value),
            "GroupId": parent.CandidateId,
            "Children": []
        });
        $(option).addClass('node-secondary');
    }
    o.Finalize();
}
function removefromList() {
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