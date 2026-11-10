# Reference provenance and scene mapping

The supplied concept deck, `Candidate Concept_1.pptx`, is the project concept source. Slide 2 specifies the sequence Interior Mission Area / Airlock / Exterior Maintenance Area and the major props. The relevant images are included below; the original deck is maintained separately.

- `blockout-plan.png`: group-09 blockout layout authored for A1 on 2026-11-09, based on that sequence. Scale and dimensions are proposed design decisions, not measurements from the supplied deck. This is the primary spatial reference used to build the scene.
- `concept-hatch.png`: extracted verbatim from the supplied deck (ppt/media/image1.png), visual cue for framed hatches.
- `concept-maintenance.jpeg`: extracted verbatim from the supplied deck (ppt/media/image2.jpeg), visual cue for equipment and EVA maintenance.
- `concept-debris.jpeg`: extracted verbatim from the supplied deck (ppt/media/image3.jpeg), visual cue for solar arrays, Earth and debris.

The deck supplies no original image URLs or rights information. Embedded images remain the property of their respective owners, are retained only as supplied course-design references and are not shipped as game textures.

## Comparison with scene

| Plan feature | Unity hierarchy | Dimensions / location |
| --- | --- | --- |
| Mission area | Environment/01_MissionArea | 8 x 12 m, z=0..12 |
| Tools, terminal, storage | Children of 01_MissionArea | Perimeter positions shown on plan |
| Airlock | Environment/02_Airlock | 4 x 4 m, z=12..16 |
| Open hatches | Hatch_Interior / Hatch_Exterior | 2.4 x 2.6 m clear opening |
| Exterior maintenance | Environment/03_Exterior | 12 x 12 m, z=16..28 |
| Solar silhouettes | 03_Exterior/Solar_Left, Solar_Right | Outside side rails, non-walkable |
| Repair, bin, debris | Children of 03_Exterior | Perimeter and off the central route |
| Earth placeholder | Context/Earth_ScalePlaceholder | Distant flat sphere, no final texture |

The walkable exterior deck and rails are deliberate A1 inspection aids. Later VR work may replace them with handrail locomotion. Hatches remain open, props remain static, and repair/grabbing are not implemented in A1.
