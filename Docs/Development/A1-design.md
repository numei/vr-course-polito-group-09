# Orbital Rescue A1 design

## Intent and scope
Implement the first spatial prototype of the supplied Orbital Rescue concept in the group repository. A1 is a desktop first-person walkthrough, before VR locomotion, physics interactions and the A2 art pass. Use primitive volumes, flat identification colours and neutral preview lighting.

## Reference and layout
The supplied Candidate Concept_1.pptx describes an interior mission area, an airlock and an exterior maintenance area, in that order. Its embedded images are visual references for hatches, maintenance equipment, solar panels, Earth and debris; they do not define dimensions. Save those images in Docs/References. Add an explicitly authored dimensioned plan, before constructing the scene, to make the layout reproducible and comparable.

Builder's scale decisions: 1 Unity unit = 1 m. Interior x=-4..4, z=0..12, ceiling 3.6 m. Airlock x=-2..2, z=12..16, ceiling 3.2 m. Exterior deck x=-6..6, z=16..28. Two open hatches have 2.4 m clear width and 2.6 m clear height. Place mission terminal, tool rack and storage at the interior perimeter. Put repair interface, debris collection bin, debris volumes and solar array silhouettes around a continuous exterior inspection route. Rails bound the A1 deck. All walkable zones share a floor level. A plain distant sphere stands in for Earth.

## Approach
Use the installed Unity 6000.4.4f1 (newer than the required Unity 6.3), URP and a custom CharacterController driven by a serialized InputActionAsset. Primitive GameObjects keep the blockout directly editable and avoid unnecessary ProBuilder dependencies (ProBuilder is recommended, not mandatory). Starter Assets would add unrelated sample content and Cinemachine; a runtime-only procedural environment would prevent normal scene inspection. Therefore save actual geometry in Assets/Scenes/Blockout.unity, with an explicit editor regeneration command and a reusable player prefab.

## Controller contract
WASD/arrows move relative to yaw, mouse looks, Shift sprints, Escape releases capture, left click resumes, R returns to spawn. Also support gamepad sticks, left-stick sprint, Start release/resume and Select reset. Normalize diagonal movement. Separate mouse delta sensitivity from gamepad degrees per second. Clamp camera pitch. CharacterController gravity, wall collisions and stable grounded movement must work. Pause movement when cursor is released or application focus is lost. Reset must restore pose and vertical velocity. The child camera has a 1.65 m eye height and exactly one AudioListener.

## Delivery and evidence
Project at repository root, branch assignment-1, root Docs reference images, README with internal name, synopsis and A0 working-document link. Do not invent an A0 URL or claim a collaborator was added without evidence. Validate import/compile, saved scene, controller input, collisions and complete traversal with Unity tests, then inspect the actual rendered scene and Play mode. Validate a fresh checkout without Library. Commit and push the requested branch if authentication allows; verify its remote SHA. Record any remaining external submission requirements honestly.
