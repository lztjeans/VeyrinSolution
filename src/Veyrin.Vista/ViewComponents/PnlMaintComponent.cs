using Microsoft.AspNetCore.Mvc;


[ViewComponent(Name = "PnlMaint")]
public class PnlMaintComponent : ViewComponent
{
    public PnlMaintComponent() { }

    public IViewComponentResult Invoke(BOmMaint maint)
    {
        ViewData["Type"] = maint;
        ViewData["Header"] = GenHeader(maint);
        //ViewData["AttrNm"] = GenAttrNm(maint);
        ViewData["Elements"] = GenElements(maint);
        return View();
    }
    private static string? GenHeader(BOmMaint maint)
    {
        string header = string.Empty;
        switch (maint)
        {
            case BOmMaint.WLCSPPT:
                header = "WLCSP Plating"; break;
            case BOmMaint.WLCSPRDL:
                header = "WLCSP RDL"; break;
            case BOmMaint.WLCSPUBM:
                header = "WLCSP UBM"; break;
            default:
                break;
                //case BOmMaint.LF:
                //    header = "Lead Frame"; break;
                //case BOmMaint.PT:
                //    header = "Plating"; break;
                //case BOmMaint.SS:
                //    header = "Substrate"; break;
                //case BOmMaint.SB:
                //    header = "Solder Ball"; break;
                //case BOmMaint.MDC:
                //    header = "Molding Compound"; break;
                //case BOmMaint.PK:
                //    header = "Packing"; break;
                //case BOmMaint.DABW:
                //    header = "Die Attach & Bonding Wire"; break;
                //case BOmMaint.Copt:
                //    header = "Component"; break;
                //case BOmMaint.SPCR:
                //    header = "Spacer"; break;
                //case BOmMaint.WLCSPBP:
                //    header = "WLCSP Bump"; break;
                //case BOmMaint.WLCSPSB:
                //    header = "WLCSP Solder Ball"; break;
                //case BOmMaint.WLCSPPSV:
                //    header = "WLCSP Passivation"; break;
                //case BOmMaint.BSC:
                //    header = "BackSide Coating"; break;
                //case BOmMaint.Other:
        }
        return header;
    }

    private static List<AbstractElements> GenElements(BOmMaint maint)
    {
        var result = CollectionUtils.CreateList<AbstractElements>();
        switch (maint)
        {
            case BOmMaint.WLCSPRDL:
                result.Add(SelectElement.Create(id: "cbRDLLayer_WLCSP", label: "RDL Layer"));
                break;
            case BOmMaint.WLCSPUBM:
                result.Add(SelectElement.Create(id: "cbUBMSize_WLCSP", label: "UBM Size (um)"));
                break;
            default:
                break;
        }
        return result;
        //case BOmMaint.LF:
        //    result.Add(Element.CreateComboBox(id: "LFType", label: "LF Type"));
        //    result.Add(Element.CreateComboBox(id: "LFMaterial", label: "LF Material"));
        //    result.Add(Element.CreateComboBox(id: "LFVendor", label: "LF Vendor"));
        //    result.Add(Element.CreateComboBox(id: "LFProcess", label: "LF Process"));
        //    result.Add(Element.CreateTextBox(id: "LFThickness", label: "LF Thickness (mil)"));
        //    result.Add(Element.CreateComboBox(id: "LFPlatingMaterial", label: "LF Plating Material"));
        //    result.Add(Element.CreateTextBox(id: "LFSize", label: "LF Size"));
        //    result.Add(Element.CreateTextBox(id: "LF_ID", label: "LF ID"));
        //    result.Add(Element.CreateComboBox(id: "ExposedPad", label: "Exposed Pad"));
        //    result.Add(Element.CreateTextBox(id: "StripUnit_LF", label: "Strip Unit"));
        //    //result.Add(Element.CreateComboBox(id: "LF_ID", label: "LF ID"));
        //    //result.Add(Element.CreateComboBox(id: "StripUnit_LF", label: "Strip Unit"));
        //    break;
        //case BOmMaint.PT:
        //    result.Add(Element.CreateComboBox(id: "PlatingVendor", label:"Plating Vendor"));
        //    result.Add(Element.CreateComboBox(id: "PlatingMaterial", label:"Plating Material"));
        //    result.Add(Element.CreateComboBox(id: "PlatingType", label:"Plating Type"));
        //    break;
        //case BOmMaint.SS:
        //    result.Add(Element.CreateComboBox(id: "SubstrateLayer", label: "Substrate Layer"));
        //    result.Add(Element.CreateComboBox(id: "BallSolderableSurfaceFinger", label: "Ball Solderable Surface/Finger"));
        //    result.Add(Element.CreateComboBox(id: "BTType", label: "BT Type"));
        //    result.Add(Element.CreateComboBox(id: "BTVendor", label: "BT Vendor"));
        //    result.Add(Element.CreateComboBox(id: "SolderMaskType", label: "Solder Mask Type"));
        //    result.Add(Element.CreateComboBox(id: "SolderMaskVendor", label: "Solder Mask Vendor"));
        //    result.Add(Element.CreateTextBox(id: "Substrate_ID", label: "Substrate ID"));
        //    result.Add(Element.CreateComboBox(id: "SubstrateVendor", label: "Substrate Vendor"));
        //    result.Add(Element.CreateTextBox(id: "SubstrateThickness", label: "Substrate Thickness"));
        //    result.Add(Element.CreateTextBox(id: "StripUnit_SUB", label: "Strip Unit"));
        //    //result.Add(Element.CreateComboBox(id: "Substrate_ID", label: "Substrate ID"));
        //    //result.Add(Element.CreateComboBox(id: "SubstrateThickness", label: "Substrate Thickness"));
        //    //result.Add(Element.CreateComboBox(id: "StripUnit_SUB", label: "Strip Unit"));
        //    break;
        //case BOmMaint.SB:
        //    result.Add(Element.CreateComboBox(id: "SBComposition", label: "S/B Composition"));
        //    result.Add(Element.CreateComboBox(id: "SBVendor", label: "S/B Vendor"));
        //    result.Add(Element.CreateComboBox(id: "SBType", label: "S/B Type"));
        //    result.Add(Element.CreateComboBox(id: "SBDiameter", label: "S/B Diameter"));
        //    result.Add(Element.CreateComboBox(id: "SBPhosphorus", label: "Phosphorus"));
        //    break;
        //case BOmMaint.MDC:
        //    result.Add(Element.CreateComboBox(id: "MCVendor", label: "M/C Vendor"));
        //    result.Add(Element.CreateComboBox(id: "MCType", label: "M/C Type"));
        //    result.Add(Element.CreateComboBox(id: "AlphaRayEmission", label: "Alpha Ray Emission"));
        //    break;
        //case BOmMaint.PK:
        //    result.Add(Element.CreateTextBox(id: "PackingNumber", label: "Packing Number"));
        //    result.Add(Element.CreateComboBox(id: "PackingVendor", label: "Packing Vendor"));
        //    result.Add(Element.CreateComboBox(id: "PackingType", label: "Packing Type"));
        //    result.Add(Element.CreateComboBox(id: "PackingMaterial", label: "Packing Material"));
        //    break;
        //case BOmMaint.DABW:
        //    result.Add(Element.CreateComboBox(id: "cbMcpStructure", label: "MCP Structure"));
        //    break;
        //case BOmMaint.SPCR:
        //    result.Add(Element.CreateComboBox(id: "cbSpcrStructure", label: "Spacer Structure"));
        //    break;
        //case BOmMaint.Copt:
        //    result.Add(Element.CreateButon(id: "btnAddCopt", label: "+ Add",eventNm: "btnAddCopt_Click();"));
        //    result.Add(Element.CreateButon(id: "btnDelCopt", label: "- Delete", eventNm: "btnDelCopt_Click();"));
        //    break;
        //case BOmMaint.WLCSPBP:
        //    result.Add(Element.CreateComboBox(id: "BallMntProcTypeItems", label: "Ball Mount Process"));
        //    result.Add(Element.CreateComboBox(id: "BallPitch", label: "Ball Pitch (um)"));
        //    result.Add(Element.CreateComboBox(id: "BumpHeightNom", label: "Bump Height Nom."));
        //    break;
        //case BOmMaint.WLCSPSB:
        //    result.Add(Element.CreateComboBox(id: "SBComposition_WLCSP", label: "S/B Composition"));
        //    result.Add(Element.CreateComboBox(id: "SBVendor_WLCSP", label: "S/B Vendor"));
        //    result.Add(Element.CreateComboBox(id: "SBType_WLCSP", label: "S/B Type"));
        //    result.Add(Element.CreateComboBox(id: "SBDiameter_WLCSP", label: "S/B Diameter (mm)"));
        //    result.Add(Element.CreateComboBox(id: "SBPhosphorus_WLCSP", label: "S/B Phosphorus (P)"));
        //    break;
        //case BOmMaint.WLCSPPSV:
        //    result.Add(Element.CreateComboBox(id: "PassivationVendor_WLCSP", label: "Passivation Vendor"));
        //    result.Add(Element.CreateComboBox(id: "PassivationMaterial_WLCSP", label: "Passivation Material"));//?
        //    result.Add(Element.CreateComboBox(id: "PassivationType_WLCSP", label: "Passivation Type"));
        //    break;
        //case BOmMaint.BSC:
        //    result.Add(Element.CreateComboBox(id: "BscTypeItems", label: "Backside Coating"));
        //    result.Add(Element.CreateTextBox(id: "BacksideCoatingVendor", label: "Backside Coating Vendor"));
        //    result.Add(Element.CreateComboBox(id: "BacksideCoatingType", label: "Backside Coating Type"));
        //    result.Add(Element.CreateComboBox(id: "BacksideCoatingThickness", label: "Backside Coating Thickness (um)"));
        //    result.Add(Element.CreateComboBox(id: "BacksideCoatingMaterial", label: "Backside Coating Material"));
        //    break;
        //case BOmMaint.Other:
        //    break;

    }


}
