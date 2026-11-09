# A1 requirement-by-requirement checklist

Checked against the full [official A1 page](https://cgvg-polito.notion.site/A1-3edc27e51e0f8189bbbcfec70f0194d8), read on 9 November 2026. Deadline: **15 November 2026 at 23:59 Europe/Rome**. This file distinguishes local implementation evidence from external delivery checks.

| Official requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Repository named `vr-course-polito-group-XX` | PASS | `numei/vr-course-polito-group-09`, confirmed using GitHub repository metadata. |
| Teacher `francescoStrada` added as collaborator | UNVERIFIED | GitHub connector returned 403 for collaborator permissions; browser has no signed-in session. Repository owner must check Settings → Collaborators and add/confirm the teacher. No invitation has been sent by this implementation. |
| Branch `assignment-1` | LOCAL PASS; remote pending | Local branch is `assignment-1`. GitHub initially has only `main`; final push is verified below when performed. |
| Unity project at repository root | PASS | Root `Assets/`, `Packages/`, `ProjectSettings/`. |
| README includes internal project name and synopsis | PASS | Orbital Rescue, intended astronaut maintenance / debris-removal experience. |
| README links to A0 working document | DRAFT LINK PRESENT | README links to `Docs/A0-working-document.md`, seeded from the supplied concept deck. The user has not confirmed an existing shared A0 URL. Group should replace the link if it already has an official working document. No claim of final A0 completion. |
| At least one reference image in root `Docs` folder | PASS | `Docs/References/blockout-plan.png` plus three unmodified images extracted from the concept deck, original PPTX and provenance. |
| Blockout based on a reference | PASS | Dimensioned plan authored from slide 2's mission-area / airlock / exterior sequence; layout and props mapped in `Docs/References/README.md`. |
| Simple volumes establishing scale, layout and spatial flow | PASS | Primitive walls, floors, open hatches, equipment and railings. 1 unit = 1 m; 1 m grid; documented dimensions; entire route navigable. |
| No final art; assets and lighting later | PASS | Flat materials, neutral preview lights, static interaction placeholders, plain Earth sphere. No reference images used as textures. |
| ProBuilder recommended | OPTIONAL | Editable primitives used. No ProBuilder dependency or missing pipeline support materials. |
| Opens in Unity 6.3 or newer without project errors | PASS (local); fresh checkout pending | Imported and compiled in Unity **6000.4.4f1**. Scene build and test commands exit successfully. Final clean-checkout result recorded below. |
| Saved scene `Assets/Scenes/Blockout.unity` matches reference | PASS | Serialized editable scene, not a runtime-only generator. Actual rendered interior, airlock, exterior and overview views in `Docs/Validation/`. Overview hides ceilings only for the image. |
| First-person controller with camera | PASS | `Player.prefab`: CharacterController, controller, child MainCamera at 1.65 m and one AudioListener. |
| New Input System, not legacy Input Manager | PASS | Input System 1.17, `.inputactions` asset, `activeInputHandler: 1`; controller uses InputActions. |
| Press Play, move and look around the whole space | PASS (automated); native Play check pending | Full suite: 4/4 EditMode and 10/10 PlayMode tests passed. Tests include full-height central route, both-hatch return trip, wall / exterior collision, WASD, mouse yaw / pitch, diagonal normalization, gamepad, focus loss / resume, reset, 30/120 FPS stick look. |
| VR locomotion is not required at A1 | PASS | Desktop walkthrough. No XR runtime or headset is required. |
| Clone into new empty folder and open | PENDING | Will verify committed source in a new directory with no existing `Library`. |
| Last commit pushed on `assignment-1` before deadline | PENDING | Local commit alone is insufficient. Remote SHA and verification time must be recorded after push. |
| Check last commit on GitHub | PENDING | Compare remote `refs/heads/assignment-1` to local HEAD after push. |
| Do not push to `assignment-1` after deadline | DOCUMENTED | README states to use a new branch after the deadline. |

## Automated verification

`./Tools/verify.sh` runs the complete EditMode and PlayMode suites. Local result: **14 tests passed, 0 failed, 0 skipped**. The script checks XML presence and nonempty passing results. Full logs and XML are local in ignored `TestResults/`.

The initial tests were observed failing with the scene/action asset missing. Keyboard/mouse batch-mode tests initially revealed the editor focus routing default; the test fixture uses Unity's documented editor input routing option only during tests and restores the prior settings. Normal product input settings remain unchanged.

## Final verification record

Pending final clean-checkout, native Play and remote commit verification. Do not treat pending or unverified external items above as passed.
