const marquee = document.querySelector('.marquee_container');
const marqueeContent = marquee.querySelector('.marquee_content');

const marqueeContentDupe = marqueeContent.cloneNode(true);
marquee.appendChild(marqueeContentDupe)

console.log(marqueeContentDupe);

let tween;

const playMarquee = () => {
    let progress = tween ? tween.progress() : 0;
    tween && tween.progress(0).kill();
    const width = parseInt(getComputedStyle(marqueeContent).getPropertyValue('width'), 10);

    const gap = parseInt(getComputedStyle(marqueeContent).getPropertyValue('column-gap'), 10);

    const distanceToTranslate = -1 * (gap + width);

    tween = gsap.fromTo(marquee.children, { x: 0 }, {
        x: distanceToTranslate, duration: 8, repeat: -1, ease: 'none'
    })
    tween.progress(progress)
}

playMarquee()

function debounce(func) {
    var timer;
    return function (event) {
        if (timer) clearTimeout(timer)
        timer = setTimeout(
            () => {
                func();
            }, 500, event)
    }
}



window.addEventListener('resize', debounce(playMarquee))