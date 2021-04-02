window.isoMosaic = function (cgrid) {
	var dgrid;
	if (cgrid) {
		if (cgrid.nodeType === 1 && cgrid.tagName === "div" && cgrid.hasAttribute('isoMosaic')) {
			dgrid = [cgrid];		
		}
		else {
			return;
		}
	}
	else {
		dgrid = Array.from(document.querySelectorAll('[isoMosaic]'));		
	}

	function loadImages(tub,grid,onAllLoaded) {
		var i, g, ln, files=grid.children,numLoading = files.length;
		const onload = function() {
			--numLoading === 0 && onAllLoaded(tub,grid,files);
		}
		const images = {};
		for (i = 0; i < files.length; i++) {
			const img = images['files'+i] = new Image;
			if (files[i].ld!==1) {
				files[i].style.display='none';
				files[i].style.position='absolute';
			}
			if (files[i].hasAttribute('isosrc')) {
				ln = files[i].getAttribute('isosrc').split(',');
				for (g = 0; g < ln.length; g++) {
					const imglst = images['files'+i+'_'+g] = new Image;
					imglst.src = ln[g];
					imglst.onload = onload;
					imglst.onerror = onload;
					numLoading++;
				}
				--numLoading;
			}
			else if (files[i].hasAttribute('src')) {
				img.src = files[i].getAttribute('src');
				img.onload = onload;
				img.onerror = onload;				
			}
			else {
				onload();
			}
		}
		return images;
	}

	function getDistribution(setSize, max, min) {
		var distribution = [],
			processed = 0;
		while (processed < setSize) {
			if (setSize - processed <= max && processed > 0) {
				distribution.push(setSize - processed);
				processed += setSize - processed;
			}
			else {
				var current = Math.max(Math.floor(Math.random() * max + 1), min);
				current = Math.min(current, setSize - processed);
				distribution.push(current);
				processed += current;
			}
		}
		return distribution;
	}

	function arrayAlternate(array, remainder) {
		return array.filter(function (value, index) {
			return index % 2 === remainder;
		});
	}

	function inArray(valToFind, array, fromIndex) {
		for (var index = (fromIndex ? fromIndex : 0); index < array.length; index++) {
			if (array[index] === valToFind) {
				return index;
			}
		}
		return -1;
	}

	function setUniformHeightsForRow(array) {
		array.sort(function (a, b) {
			return a.height - b.height;
		});
		array[0].new_height = array[0].height;
		array[0].new_width = array[0].width;

		for (var i = 1; i < array.length; i++) {
			array[i].new_height = array[0].height;
			array[i].new_width = array[i].new_height * array[i].aspect_ratio;
		}
		var new_width = array.reduce(function (sum, p) {
			return sum += p.new_width;
		}, 0);
		return {
			elements: array,
			height: array[0].new_height,
			width: new_width,
			aspect_ratio: new_width / array[0].new_height
		};
	}

	function finalizeTiledLayout(components, containers, bX, bY, gutter) {
		var cLength = components.length,
			c;
		for (c = 0; c < cLength; c++) {
			var component = components[c],
				e,
				g = gutter,
				g2 = gutter / 2,
				rowY = component.y,
				otherRowHeight = 0,
				container,
				g2X=g2 + bX,
				g2Y=g2 + bY,
				ceLen = component.elements.length;
			for (e = 0; e < ceLen; e++) {
				var element = component.elements[e];
				if (element.photo_position !== undefined) {
					(container = containers[element.photo_position].style).width = (component.new_width - g) + 'px';
					container.height = (component.new_height - g) + 'px';
					container.top = (component.y + g2Y) + 'px';
					container.left = (component.x + g2X) + 'px';
				}
				else {
					element.new_width = component.new_width;
					if (otherRowHeight === 0) {
						element.new_height = element.new_width / element.aspect_ratio;
						otherRowHeight = element.new_height;
					}
					else {
						element.new_height = component.new_height - otherRowHeight;
					}
					element.x = component.x;
					element.y = rowY;
					rowY += element.new_height;
					var totalWidth = element.elements.reduce(function (sum, p) {
						return sum += p.new_width;
					}, 0);
					var rowX = 0,
						i,
						eLength = element.elements.length;
					for (i = 0; i < eLength; i++) {
						var image = element.elements[i];
						image.new_width = element.new_width * image.new_width / totalWidth;
						image.new_height = element.new_height;
						image.x = rowX;
						rowX += image.new_width;
						(container = containers[image.photo_position].style).width = Math.floor(image.new_width - g) + 'px';
						container.height = Math.floor(image.new_height - g) + 'px';
						container.top = Math.floor(element.y + g2Y) + 'px';
						container.left = Math.floor(element.x + image.x + g2X) + 'px';
					}
				}
			}
		}
	}

	var cb = function (tub2,grid,images) {
		var idx = 0,
			bX0 = tub2.bufferX,
			bY0 = tub2.bufferY,
			bX = (bX0 || (bX0===0))?bX0:0,
			bY = (bY0 || (bY0===0))?bY0:0,
			gutter = (tub2.gutter || (tub2.gutter===0))?tub2.gutter:1,
			i,
			viewportWidth = Math.floor(grid.offsetWidth - bX - bX),
			triggerWidth = (tub2.complex?tub2.complex:8)*25,
			maxInRow = Math.floor(viewportWidth / triggerWidth),
			minInRow = viewportWidth >= (triggerWidth * 2) ? 2 : 1,
			photos = [],
			containers = [],
			imgIdx = 0,
			div,w,h,
			setSize = images.length;

		if (setSize === 0) {
			return;
		}

		for (i = 0; i < images.length; i++) {
			if (images[i].ld!==1) {
				images[i].ld=1;
				images[i].style.display='block';
			}
		}

		for (let image of images) {
			containers[imgIdx] = image;
			if (image.hasAttribute('isodim')) {
				i=image.getAttribute('isodim').split(',');
				if (i.length===2) {
					w=parseInt(i[0]);
					h=parseInt(i[1]);
				}
				else {
					w=image.naturalWidth;
					h=image.naturalHeight;
					image.setAttribute('isodim',w+','+h);
				}
			}
			else {
				if (!(image.nodeType === 1 && image.tagName === "img")) {
					var img=new Image;
					img.src=image.getAttribute('src');
					w=img.naturalWidth;
					h=img.naturalHeight;
				}
				else {
					w=image.naturalWidth;
					h=image.naturalHeight;
				}
				image.setAttribute('isodim',w+','+h);
			}
			if (!(h === 0 || h === undefined || w === undefined)) {
				photos.push({
					src: image.src,
					width: w,
					height: h,
					aspect_ratio: w / h,
					photo_position: imgIdx
				});
			}
			imgIdx++;
		};

		setSize = photos.length;
		var distribution = getDistribution(setSize, maxInRow, minInRow),
			groups = [],
			startIdx = 0,
			groupY = 0,
			g;

		for (let size of distribution) {
			groups.push(photos.slice(startIdx, startIdx + size));
			startIdx += size;
		};

		for (g = 0; g < groups.length; g++) {
			var group = groups[g],
				groupLayout;

			group.sort(function (a, b) {
				return a.aspect_ratio - b.aspect_ratio;
			});

			if (group.length === 1) {
				groupLayout = [1];
			}
			else if (group.length === 2) {
				groupLayout = [1,1];
			}
			else {
				groupLayout = getDistribution(group.length, group.length - 1, 1);
			}

			var cliqueF = 0,
				cliqueL = group.length - 1,
				cliques = [],
				indices = [],
				i,
				clique = [],
				j;

			for (i = 2; i <= maxInRow; i++) {
				var index = inArray(i, groupLayout);
				while (-1 < index && cliqueF < cliqueL) {
					clique = [];
					j = 0;
					while (j < i && cliqueF <= cliqueL) {
						clique.push(group[cliqueF++]);
						j++;
						if (j < i && cliqueF <= cliqueL) {
							clique.push(group[cliqueL--]);
							j++;
						}
					}
					cliques.push(clique);
					indices.push(index);
					index = inArray(i, groupLayout, index + 1);
				}
			}

			var remainder = group.slice(cliqueF, cliqueL + 1),
				c, wide, rowLayout = [],
				cL = cliques.length,
				oneRow, otherRow;

			for (c = 0; c < cL; c++) {
				var clique = cliques[c];

				if (Math.floor(Math.random() * 2) === 0) {
					wide = Math.max(Math.floor(Math.random() * (clique.length / 2 - 1)), 1);
					oneRow = clique.slice(0, wide);
					otherRow = clique.slice(wide);
				}
				else {
					oneRow = arrayAlternate(clique, 0);
					otherRow = arrayAlternate(clique, 1);
				}
				oneRow = setUniformHeightsForRow(oneRow);
				otherRow = setUniformHeightsForRow(otherRow);

				oneRow.new_width = Math.min(oneRow.width, otherRow.width);
				oneRow.new_height = oneRow.new_width / oneRow.aspect_ratio;
				otherRow.new_width = oneRow.new_width;
				otherRow.new_height = otherRow.new_width / otherRow.aspect_ratio;

				rowLayout.push({
					elements: [oneRow, otherRow],
					height: oneRow.new_height + otherRow.new_height,
					width: oneRow.new_width,
					aspect_ratio: oneRow.new_width / (oneRow.new_height + otherRow.new_height),
					element_position: indices[c]
				});
			}

			rowLayout.sort(function (a, b) {
				return a.element_position - b.element_position;
			});

			var position, orderedRowLayout = [],
				rem;

			for (position = 0; position < groupLayout.length; position++) {
				if (inArray(position, indices) > -1) {
					orderedRowLayout.push(rowLayout.shift());
				}
				else {
					rem = remainder.shift();
					orderedRowLayout.push({
						elements: [rem],
						height: rem.height,
						width: rem.width,
						aspect_ratio: rem.aspect_ratio
					});
				}
			}

			var totalAspect = orderedRowLayout.reduce(function (sum, p) {
				return sum += p.aspect_ratio;
			}, 0);

			var elementX = 0;
			for (let component of orderedRowLayout) {
				component.new_width = component.aspect_ratio / totalAspect * viewportWidth;
				component.new_height = component.new_width / component.aspect_ratio;
				component.y = groupY;
				component.x = elementX;
				elementX += component.new_width;
			};

			groupY += orderedRowLayout[0].new_height;
			finalizeTiledLayout(orderedRowLayout, containers, bX, bY, gutter);
		}

		grid.style.height = (groupY+bY) + 'px';
	};

	var i,tub2,ndef,defs;
	for (i=0;i<dgrid.length;i++) {
		ndef={};
		defs=dgrid[i].getAttribute('isoMosaic').split(',');
		if (defs.length==4) {
			ndef.bufferX=parseInt(defs[0]);
			ndef.bufferY=parseInt(defs[1]);
			ndef.gutter=parseInt(defs[2]);
			ndef.complex=parseInt(defs[3]);
		}
		loadImages((ndef==={}?this.defs:ndef),dgrid[i],cb);
	}

};

isoMosaic.defs={
	bufferX: 0,
	bufferY: 0,
	gutter: 4,
	complex : 8
};

window.addEventListener("resize", function() {isoMosaic();});
document.addEventListener("DOMContentLoaded",function(){
	isoMosaic();
});