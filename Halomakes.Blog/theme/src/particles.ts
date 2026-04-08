import {tsParticles, type Container} from "@tsparticles/engine";
import {loadTrianglesPreset} from "@tsparticles/preset-triangles"

declare let window: { particles?: Container };

(async () => {
    await loadTrianglesPreset(tsParticles);

    window.particles = await tsParticles.load({
        id: "tsparticles",
        options: {
            preset: "triangles",
            particles: {
                move: {
                    speed: {min: .003, max: .2}
                },
                number: {
                    density: {
                        enable: true,
                        width: 1920,
                        height: 1080
                    }
                }
            },
            fpsLimit: 60
        },
    });
})();