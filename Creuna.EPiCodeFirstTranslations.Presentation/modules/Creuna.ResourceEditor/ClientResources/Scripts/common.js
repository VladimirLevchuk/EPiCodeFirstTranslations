$(function() {
	$('.todo').append('<div class="hardcodedBorder">&nbsp;</div>');
//Caret function

	  $.fn.caret = function(pos) {
    var target = this[0];
	var isContentEditable = target.contentEditable === 'true';
    //get
    if (arguments.length == 0) {
      //HTML5
      if (window.getSelection) {
        //contenteditable
        if (isContentEditable) {
          target.focus();
          var range1 = window.getSelection().getRangeAt(0),
              range2 = range1.cloneRange();
          range2.selectNodeContents(target);
          range2.setEnd(range1.endContainer, range1.endOffset);
          return range2.toString().length;
        }
        //textarea
        return target.selectionStart;
      }
      //IE<9
      if (document.selection) {
        target.focus();
        //contenteditable
        if (isContentEditable) {
            var range1 = document.selection.createRange(),
                range2 = document.body.createTextRange();
            range2.moveToElementText(target);
            range2.setEndPoint('EndToEnd', range1);
            return range2.text.length;
        }
        //textarea
        var pos = 0,
            range = target.createTextRange(),
            range2 = document.selection.createRange().duplicate(),
            bookmark = range2.getBookmark();
        range.moveToBookmark(bookmark);
        while (range.moveStart('character', -1) !== 0) pos++;
        return pos;
      }
      //not supported
      return 0;
    }
    //set
    if (pos == -1)
      pos = this[isContentEditable? 'text' : 'val']().length;
    //HTML5
    if (window.getSelection) {
      //contenteditable
      if (isContentEditable) {
        target.focus();
        window.getSelection().collapse(target.firstChild, pos);
      }
      //textarea
      else
        target.setSelectionRange(pos, pos);
    }
    //IE<9
    else if (document.body.createTextRange) {
      var range = document.body.createTextRange();
      range.moveToElementText(target);
      range.moveStart('character', pos);
      range.collapse(true);
      range.select();
    }
    if (!isContentEditable)
      target.focus();
    return pos;
  }
	
	
	
	
	
	
	$('body').on('click', '.textContent', function() {
			var actualText = $(this).html();
			var editArea = $(this).next();
			//var replacedText = actualText.replace(/<br>/g, '\r\n');
			
			
			$('.textContent').removeClass('hide');
			$('.editeble').removeClass('show');
			
			$(this).addClass('hide');
			//editArea.addClass('show').val(replacedText).focus().caret(-1);
			editArea.addClass('show').focus().caret(-1);

	});


	$('body').on('focusout', '.editeble', function() {
		var editedText = $(this).val();
		var replacedText = editedText.replace(/\r\n|\r|\n/g,'<br />');
		var edit = $(this);
		var text = $(this).prev();
		
		setTimeout(function() {
			//text.removeClass('hide').html(replacedText);
			text.removeClass('hide');
			edit.removeClass('show');
			
		}, 
		
		100);	
	});
	
	
	$('body').on('keydown', '.editeble', function(e) {
		var nextItem = $(this).parent().parent().next('tr');
		var textNext = nextItem.find('.textContent');
		
		var prevItem = $(this).parent().parent().prev();
		var textPrev = prevItem.find('.textContent');
		
		var actualText = textPrev.html();
		var activeEdit = $(this);
		var caret = activeEdit.caret()
		var content = activeEdit.val();

		
		if (e.ctrlKey && e.keyCode == 13) {
			this.value = content.substring(0,caret)+"\n"+content.substring(caret,content.length);
			return false;
		}
		
		if(prevItem.length){
			if (e.shiftKey && e.keyCode === 9) {
				textPrev.click();
				return false;
			}
		}
		else{
			if (e.shiftKey && e.keyCode === 9) {
				$(this).focusout();
				return false;
			}
		}
		
		
		if(nextItem.length){
			if( e.keyCode === 13 || e.keyCode === 9) {
				textNext.click();
				return false;
			}
			
		}
		else{
			if( e.keyCode === 13 || e.keyCode === 9) {
				$(this).focusout();
				return false;
			}
			if (e.ctrlKey && e.keyCode == 13) {
				this.value = content.substring(0,caret)+"\n"+content.substring(caret,content.length);
				return false;
			}
		}
		
	});
	
	$('body').on('focus', '.textContent', function(e) {
		$(this).click();
	});
	
	
	/*$('body').on('click', '.simple-advanced span', function() {
		$('.simple-advanced span').toggleClass('active');
		$('.translation-block').toggleClass('advanced');
	});*/
	
	
	
	
});