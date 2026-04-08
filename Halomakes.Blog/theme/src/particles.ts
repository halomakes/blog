import {tsParticles} from "@tsparticles/engine";
import {loadTrianglesPreset} from "@tsparticles/preset-triangles"

(async () => {
    await loadTrianglesPreset(tsParticles);

    await tsParticles.load({
        id: "tsparticles",
        options: {
            preset: "triangles",
            particles: {
                move: {
                    speed: {min: .001, max: .1}
                }
            }
        },
    });
})();