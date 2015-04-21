--EndMtgId
UPDATE dbo.Releases 
SET EndMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = EndMtgId)
WHERE EndMtgId IS NOT NULL;

--ClosureMtgId
UPDATE dbo.Releases 
SET ClosureMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = ClosureMtgId)
WHERE ClosureMtgId IS NOT NULL;

--Stage1
UPDATE dbo.Releases 
SET Stage1FreezeMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = Stage1FreezeMtgId)
WHERE Stage1FreezeMtgId IS NOT NULL;

--Stage2
UPDATE dbo.Releases 
SET Stage2FreezeMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = Stage2FreezeMtgId)
WHERE Stage2FreezeMtgId IS NOT NULL;

--Stage3
UPDATE dbo.Releases 
SET Stage3FreezeMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = Stage3FreezeMtgId)
WHERE Stage3FreezeMtgId IS NOT NULL;

--TsgApprovalMtgRef
UPDATE dbo.WorkItems 
SET TsgApprovalMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = TsgApprovalMtgId)
WHERE TsgApprovalMtgId IS NOT NULL;

--PcgApprovalMtgRef
UPDATE dbo.WorkItems 
SET PcgApprovalMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = PcgApprovalMtgId)
WHERE PcgApprovalMtgId IS NOT NULL;

--TsgStoppedMtgRef
UPDATE dbo.WorkItems 
SET TsgStoppedMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = TsgStoppedMtgId)
WHERE TsgStoppedMtgId IS NOT NULL;

--PcgStoppedMtgRef
UPDATE dbo.WorkItems 
SET PcgStoppedMtgRef = (SELECT vm.MtgShortRef FROM View_Meetings AS vm WHERE vm.MTG_ID = PcgStoppedMtgId)
WHERE PcgStoppedMtgId IS NOT NULL;