# A1 requirement-by-requirement checklist

Checked against the full [official A1 page](https://cgvg-polito.notion.site/A1-3edc27e51e0f8189bbbcfec70f0194d8), read on 9 November 2026. Deadline: **15 November 2026 at 23:59 Europe/Rome**. This file distinguishes local implementation evidence from external delivery checks.

| Official requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Repository named `vr-course-polito-group-XX` | PASS | `numei/vr-course-polito-group-09`, confirmed using GitHub repository metadata. |
| Teacher `francescoStrada` added as collaborator | UNVERIFIED | GitHub connector returned 403 for collaborator permissions. A browser check could open the authenticated repository settings but did not establish the teacher membership before browser control changed. Repository owner must check Settings → Collaborators and add/confirm the teacher. No invitation has been sent by this implementation. |
| Branch `assignment-1` | PASS | Local and GitHub branch `assignment-1` exist. First pushed revision `d398376768b7262715e6b75126cdf371453e30b3` matched `git ls-remote` on 9 Nov 2026 at 17:17 Europe/Rome. |
| Unity project at repository root | PASS | Root `Assets/`, `Packages/`, `ProjectSettings/`. |
| README includes internal project name and synopsis | PASS | Orbital Rescue, intended astronaut maintenance / debris-removal experience. |
| README links to A0 working document | PASS (working draft) | README links to `Docs/A0-working-document.md`, seeded from the supplied concept deck. The user has not confirmed an existing shared A0 URL. Group should replace the link if it already has an official working document. No claim of final A0 completion. |
| At least one reference image in root `Docs` folder | PASS | `Docs/References/blockout-plan.png` plus three unmodified images extracted from the concept deck, original PPTX and provenance. |
| Blockout based on a reference | PASS | Dimensioned plan authored from slide 2's mission-area / airlock / exterior sequence; layout and props mapped in `Docs/References/README.md`. |
| Simple volumes establishing scale, layout and spatial flow | PASS | Primitive walls, floors, open hatches, equipment and railings. 1 unit = 1 m; 1 m grid; documented dimensions; entire route navigable. |
| No final art; assets and lighting later | PASS | Flat materials, neutral preview lights, static interaction placeholders, plain Earth sphere. No reference images used as textures. |
| ProBuilder recommended | OPTIONAL | Editable primitives used. No ProBuilder dependency or missing pipeline support materials. |
| Opens in Unity 6.3 or newer without project errors | PASS (local + fresh checkout) | Imported and compiled in Unity **6000.4.4f1**. Scene build and test commands exit successfully. Fresh local clone also imported, compiled and passed all 14 tests. |
| Saved scene `Assets/Scenes/Blockout.unity` matches reference | PASS | Serialized editable scene, not a runtime-only generator. Actual rendered interior, airlock, exterior and overview views in `Docs/Validation/`. Overview hides ceilings only for the image. |
| First-person controller with camera | PASS | `Player.prefab`: CharacterController, controller, child MainCamera at 1.65 m and one AudioListener. |
| New Input System, not legacy Input Manager | PASS | Input System 1.17, `.inputactions` asset, `activeInputHandler: 1`; controller uses InputActions. |
| Press Play, move and look around the whole space | PASS (automated + native Play display) | Full suite: 4/4 EditMode and 10/10 PlayMode tests passed in both the original checkout and a new clone. Native Unity Editor entered Play and displayed the walkthrough correctly. Native W/S input walked from mission area to airlock, through the second hatch to the exterior, and back to mission area. Mouse look/pitch and other input behaviors passed actual Input System device-event tests. Tests include full-height central route, both-hatch return trip, wall / exterior collision, WASD, mouse yaw / pitch, diagonal normalization, gamepad, focus loss / resume, reset, 30/120 FPS stick look. |
| VR locomotion is not required at A1 | PASS | Desktop walkthrough. No XR runtime or headset is required. |
| Clone into new empty folder and open | PASS | Local `git clone --no-hardlinks --branch assignment-1` into an empty `/tmp` directory. Verified `Library` absent before first launch, then ran full verification: 14/14 passed. |
| Last commit pushed on `assignment-1` before deadline | PASS | Pushed to GitHub and verified on 9 Nov 2026, six days before the deadline. `d398376` was confirmed remotely before the final evidence-only commit. |
| Check last commit on GitHub | PASS | `git ls-remote origin refs/heads/assignment-1` matched local HEAD after push. The final evidence commit is verified again at handoff. |
| Do not push to `assignment-1` after deadline | DOCUMENTED | README states to use a new branch after the deadline. |

## Automated verification

`./Tools/verify.sh` runs the complete EditMode and PlayMode suites. Local result: **14 tests passed, 0 failed, 0 skipped**. The script checks XML presence and nonempty passing results. Full logs and XML are local in ignored `TestResults/`.

The initial tests were observed failing with the scene/action asset missing. Keyboard/mouse batch-mode tests initially revealed the editor focus routing default; the test fixture uses Unity's documented editor input routing option only during tests and restores the prior settings. Normal product input settings remain unchanged.

## Final verification record

- Source revision tested in the fresh clone: `08acb39` (implementation commit). No Library or Temp content copied.
- Clean-clone test evidence: [EditMode XML](Validation/Clean-EditMode.xml), [PlayMode XML](Validation/Clean-PlayMode.xml). 4 + 10 passed, no skipped or inconclusive tests.
- Unity performs its package API updater on first import: ShaderGraph GUID references migrate automatically for Unity 6.4. The first compiler pass emits migration diagnostics, then recompiles successfully. No project C# errors remain and both suites finish successfully. Do not mistake these first-pass migration lines for an unresolved project compilation failure.
- Native Editor opened `Blockout.unity` and entered Play with the camera, grid, props and zone/control legend visible. Automated tests drive the actual controller with Input System device events. After rebinding the native window following Play entry, actual W/S keyboard input successfully crossed both hatches to the exterior and returned to the mission area. Native mouse automation remained unavailable; mouse look is covered by the Input System tests.
- Independent read-only review found no Critical or Important issue. All scene/prefab/material GUIDs and every asset's `.meta` resolve. Original images are byte-identical to supplied PPT media.
- Native evidence: [Play mode at the exterior](Validation/05-native-play-exterior.png) and [Editor after Play, Console 0 errors / 0 warnings](Validation/06-native-editor-console.png). Unity was left with Blockout open in edit mode.
- Remote delivery verified at 17:17 Europe/Rome on 9 November 2026. Follow-up evidence-only commit is also pushed and its SHA is checked at handoff. Collaborator membership remains unverified; no pending external item is counted as passed.

## Non-blocking review notes

- `BlockoutEvidence.Capture` opens the saved scene. Save your manual scene edits before invoking it, as stated in README. Unlike the rebuild command, the capture utility does not display a save prompt.
- The installed official template's default volume profile retains obsolete Outline/Oasis subassets. URP removes unavailable components on load, and they have no effect on this blockout or its tests. They are deferred template cleanup, not scene dependencies.
