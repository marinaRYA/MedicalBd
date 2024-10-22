using PresenterLibrary.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary
{
    public class PresenterTable
    {
        private static Window _view;
        private string bd;

        public PresenterTable(Window view, string BD)
        {
            bd = BD;
            _view = view;
        }
        public IPresenterCommon ShowMedicalFacility()
        {
            var presenterMF = new PresenterMedicalFacility(_view, bd);
            return presenterMF;
        }
        public IPresenterCommon ShowSpecialization()
        {
            var presenterSpecialization = new PresenterSpecialization(_view, bd);
            return presenterSpecialization;
        }
        public IPresenterCommon ShowBuilding()
        {
            var presenterBuilding = new PresenterBuilding(_view, bd);
            return presenterBuilding;
        }
        public IPresenterCommon ShowWard()
        {
            var presenterWard = new PresenterWard(_view, bd);
            return presenterWard;
        }
        public IPresenterCommon ShowAppointment()
        {
            var presenter_app = new PresenterAppointment(_view, bd);
            return presenter_app;
        }
        public IPresenterCommon ShowDepartment()
        {
            var presenterDepartment = new PresenterDepartment(_view, bd);
            return presenterDepartment;
        }
        public IPresenterCommon ShowLaboratory()
        {
            var presenterLaboratory = new PresenterLaboratory(_view, bd);
            return presenterLaboratory;
        }
        public IPresenterCommon ShowProfileLab()
        {
            var presenterProfileLab = new PresenterProfileLab(_view, bd);
            return presenterProfileLab;
        }
        public IPresenterCommon ShowPatient()
        {
            var presenterPatient = new PresenterPatient(_view, bd);
            return presenterPatient;
        }
        public IPresenterCommon ShowSurgery()
        {
            var presenterSurgery = new PresenterSurgery(_view, bd);
            return presenterSurgery;
        }
        public IPresenterCommon ShowDoctor()
        {
            var presenterDoctor = new PresenterDoctor(_view, bd);
            return presenterDoctor;
        }
        public IPresenterCommon ShowStaff()
        {
            var presenterStaff = new PresenterStaff(_view, bd);
            return presenterStaff;
        }
        public IPresenterCommon ShowAccount()
        {
            var presenterpatientacc = new PresenterPatientAccount(_view, bd);
            return presenterpatientacc;
        }
    }
}
