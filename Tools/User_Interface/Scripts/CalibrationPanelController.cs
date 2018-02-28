using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Xml;
using Lars.Sound;

namespace Lars.UI
{

    class CalibrationPanelController : XmlLayoutController<CalibrationViewModel>
    {
        private CalibrationManager _calibMgr;
        public CalibrationManager calibMgr
        {
            get
            {
                if (_calibMgr == null)
                    _calibMgr = GameObject.Find("CalibrationManager").GetComponent<CalibrationManager>();
                return _calibMgr;
            }
        }

        public override void LayoutRebuilt(ParseXmlResult parseResult)
        {
            // ParseXmlResult.Changed   => The Xml was parsed and the layout changed as a result
            // ParseXmlResult.Unchanged => The Xml was unchanged, so no the layout remained unchanged
            // ParseXmlResult.Failed    => The Xml failed validation

            // Called whenever the XmlLayout finishes rebuilding the layout
            // Use this function to make any dynamic changes (e.g. create dynamic lists, menus, etc.) or dynamically load values/selections for elements such as DropDown
        }

        protected override void PrepopulateViewModelData()
        {
            loadViewModelData();
        }

        void loadViewModelData()
        {
            viewModel.setData(calibMgr.getData());
        }

        void playLeft()
        {
            calibMgr.PlayMaxCalibration(Channel.Left);
        }

        void playRight()
        {
            calibMgr.PlayMaxCalibration(Channel.Right);
        }

        void testLeft()
        {
            calibMgr.PlayTestCalibration(Channel.Left);
        }

        void testRight()
        {
            calibMgr.PlayTestCalibration(Channel.Right);
        }

        void saveQuit()
        {
            calibMgr.saveData();

            calibMgr.showPanel(false);
        }

        void discardQuit()
        {
            calibMgr.loadData();
            loadViewModelData();

            calibMgr.showPanel(false);
        }
    }

    public class CalibrationViewModel : XmlLayoutViewModel
    {
        CalibrationData calibData = new CalibrationData();

        public void setData(CalibrationData cd)
        {
            //if (cd == null) return;
            calibData = cd;
            refreshModel();
        }

        public void refreshModel()
        {
            MemberChanged("measuredLeft");
            MemberChanged("measuredRight");
            MemberChanged("target");
        }

        public float measuredLeft
        {
            get
            {
                return calibData.measuredAtMax_L;
            }
            set
            {
                calibData.measuredAtMax_L = value;
            }
        }

        public float measuredRight
        {
            get
            {
                return calibData.measuredAtMax_R;
            }
            set
            {
                calibData.measuredAtMax_R = value;
            }
        }

        public float target
        {
            get
            {
                return calibData.targetLevel;
            }
            set
            {
                calibData.targetLevel = value;
            }
        }
    }
}